function ConvertTo-Oid {
<#
.SYNOPSIS

Decodes a DER encoded ASN.1 object identifier (OID)

Author: Matthew Graeber (@mattifestation)
License: BSD 3-Clause

.DESCRIPTION

ConvertTo-Oid decodes a DER encoded ASN.1 object identifier (OID). This can be used as a helper function for binary certificate parsers.

.PARAMETER EncodedOIDBytes

Specifies the bytes of an absolute (starts with 6), encoded OID.

.EXAMPLE

ConvertTo-Oid -EncodedOIDBytes @(0x06, 0x0A, 0x2B, 0x06, 0x01, 0x04, 0x01, 0x82, 0x37, 0x0A, 0x03, 0x05)

.OUTPUTS

System.Security.Cryptography.Oid

ConvertTo-Oid outputs an OID object representing the decoded OID.
#>

    [OutputType([System.Security.Cryptography.Oid])]
    param (
        [Parameter(Mandatory = $True, Position = 0)]
        [Byte[]]
        [ValidateNotNullOrEmpty()]
        $EncodedOIDBytes
    )

    # This only handles absolute encoded OIDs - those that start with 6.
    # [Security.Cryptography.CryptoConfig]::EncodeOID only handles absolute OIDs.

    # This article describes the OID encoding/decoding process:
    # https://msdn.microsoft.com/en-us/library/windows/desktop/bb540809(v=vs.85).aspx

    if (($EncodedOIDBytes.Length -lt 2) -or ($EncodedOIDBytes[0] -ne 6) -or ($EncodedOIDBytes[1] -ne ($EncodedOIDBytes.Length - 2))) {
        throw 'Invalid encoded EKU OID value.'
    }

    $OIDComponents = New-Object -TypeName 'System.Collections.Generic.List[Int]'

    $SecondComponent = $EncodedOIDBytes[2] % 40
    $FirstComponent = ($EncodedOIDBytes[2] - $SecondComponent) / 40

    $OIDComponents.Add($FirstComponent)
    $OIDComponents.Add($SecondComponent)

    $i = 3

    while ($i -lt $EncodedOIDBytes.Length) {
        if (-not ($EncodedOIDBytes[$i] -band 0x80)) {
            # It is just singlebyte encoded
            $OIDComponents.Add($EncodedOIDBytes[$i])
            $i++
        } else {
            # It is either two or three byte encoded
            $Byte1 = ($EncodedOIDBytes[$i] -shl 1) -shr 1 # Strip the high bit
            $Byte2 = $EncodedOIDBytes[$i+1]

            if ($Byte2 -band 0x80) {
                # three byte encoded
                $Byte3 = $EncodedOIDBytes[$i+2]
                $i += 3

                $Byte2 = $Byte2 -band 0x7F
                if ($Byte2 -band 1) { $Byte3 = $Byte3 -bor 0x80 }
                if ($Byte1 -band 1) { $Byte2 = $Byte2 -bor 0x80 }
                $Byte2 = $Byte2 -shr 1
                $Byte1 = $Byte1 -shr 1
                if ($Byte2 -band 1) { $Byte2 = $Byte2 -bor 0x80 }
                $Byte1 = $Byte1 -shr 1

                $OIDComponents.Add([BitConverter]::ToInt32(@($Byte3, $Byte2, $Byte1, 0), 0))
            } else {
                # two byte encoded
                $i +=2

                # "Shift" the low bit from the high byte to the high bit of the low byte
                if ($Byte1 -band 1) { $Byte2 -bor 0x80 }
                $Byte1 = $Byte1 -shr 1

                $OIDComponents.Add([BitConverter]::ToInt16(@($Byte2, $Byte1), 0))
            }
        }
    }

    [Security.Cryptography.Oid] ($OIDComponents -join '.')
}
