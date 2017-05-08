<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
	<xsl:output omit-xml-declaration="no" indent="yes"/>
	<xsl:template match="*">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*|node()"/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="@*">
		<xsl:attribute name="{name()}"><xsl:value-of select="normalize-space(.)"/></xsl:attribute>
	</xsl:template>
	<xsl:template match="text()">
         <xsl:value-of select="normalize-space(.)"/>
    </xsl:template>
	<xsl:template match="*[local-name()='FuelConsumptionMap']">
		<xsl:element name="{local-name()}">
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@engineSpeed" order="ascending"/>
				<xsl:sort data-type="number" select="@torque" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='FullLoadAndDragCurve']">
		<xsl:element name="{local-name()}">
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@engineSpeed" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='TorqueLossMap']">
		<xsl:element name="{local-name()}">
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@inputSpeed" order="ascending"/>
				<xsl:sort data-type="number" select="@inputTorque" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='RetarderLossMap']">
		<xsl:element name="{local-name()}">
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@retarderSpeed" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='TorqueLimits']">
		<xsl:element name="{local-name()}">
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@gear" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='Gears']">
		<xsl:element name="{local-name()}">
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@number" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='Characteristics']">
		<xsl:element name="{local-name()}">
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@speedRatio" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='Axles']">
		<xsl:element name="{local-name()}">
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@axleNumber" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
</xsl:transform>
