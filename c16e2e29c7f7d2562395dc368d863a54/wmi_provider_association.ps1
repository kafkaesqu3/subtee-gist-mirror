<#
Author: Matthew Graeber (@mattifestation)
License: BSD 3-Clause
#>

function Get-WmiNamespace {
    [OutputType([String])]
    Param (
        [String]
        [ValidateNotNullOrEmpty()]
        $Namespace = 'ROOT',

        [Switch]
        $Recurse
    )

    $BoundParamsCopy = $PSBoundParameters
    $null = $BoundParamsCopy.Remove('Namespace')

    # Exclude locale specific namespaces
    Get-WmiObject -Class __NAMESPACE -Namespace $Namespace -Filter 'NOT Name LIKE "ms_4%"' | ForEach-Object {
        $FullyQualifiedNamespace = '{0}\{1}' -f $_.__NAMESPACE, $_.Name
        $FullyQualifiedNamespace

        if ($Recurse) {
            Get-WmiNamespace -Namespace $FullyQualifiedNamespace @BoundParamsCopy
        }
    }
}

function Get-WmiProviderAssociation {
    $UnhostedClasses = New-Object 'Collections.ObjectModel.Collection`1[System.Management.ManagementClass]'

    Get-WmiNamespace -Recurse | ForEach-Object {
        $Namespace = $_

        $ClassProviderMapping = @{}

        Get-WmiObject -Namespace $Namespace -List | % {
            if ($_.Qualifiers['Provider']) {
                $HostingProvider = $_.Qualifiers['Provider'].Value.ToLower()

                if (-not $ClassProviderMapping.ContainsKey($HostingProvider)) {
                    $ClassProviderMapping[$HostingProvider] = New-Object 'Collections.ObjectModel.Collection`1[System.Management.ManagementClass]'
                }

                $ClassProviderMapping[$HostingProvider].Add($_)
            } else {
                $UnhostedClasses.Add($_)
            }
        }

        Get-WmiObject -Namespace $_ -Class __Win32Provider | ForEach-Object {
            $ProviderCLSID = $_.CLSID
            $ClientCLSID = $_.ClientLoadableCLSID

            $ProviderImage = (Invoke-WmiMethod -Namespace root/default -Class StdRegProv -Name GetStringValue -ArgumentList @([UInt32] 2147483648, "CLSID\$ProviderCLSID\InprocServer32", $null)).sValue
            $ClientImage = (Invoke-WmiMethod -Namespace root/default -Class StdRegProv -Name GetStringValue -ArgumentList @([UInt32] 2147483648, "CLSID\$ClientCLSID\InprocServer32", $null)).sValue

            $HostedClasses = $null

            if ($ClassProviderMapping.ContainsKey($_.Name.ToLower())) {
                $HostedClasses = $ClassProviderMapping[$_.Name.ToLower()]
            }

            $Properties = [Ordered] @{
                Namespace = $Namespace
                ProviderName = $_.Name
                HostingModel = $_.HostingModel
                ProviderImage = $ProviderImage
                ClientImage = $ClientImage
                HostedClasses = $HostedClasses
            }

            New-Object -TypeName PSObject -Property $Properties
        }
    }

    # Add a catch-all entry for all classes for which there is no provider
    if ($UnhostedClasses.Count -gt 0) {
        # Create a "null" provider object
        $Properties = [Ordered] @{
            Namespace = $null
            ProviderName = $null
            HostingModel = $null
            ProviderImage = $null
            ClientImage = $null
            HostedClasses = $UnhostedClasses
        }

        New-Object -TypeName PSObject -Property $Properties
    }
}