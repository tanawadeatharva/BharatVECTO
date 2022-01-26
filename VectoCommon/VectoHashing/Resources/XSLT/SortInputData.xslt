
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
	<!--
    This XSL transformation is intended to be applied as first canonicalization step when computing the hash
    for VECTO component data or VECTO job data.

    The transformation performs the following operations:
      - strip off namespace prefixes
         (although namespace prefixes are considered part of the signature for the purpose of hashing VECTO data
         it does not provide additional semantics because the file has to validate against a XSD schema anyways 
         and may cause troubles when re-creating the VECTO data from database systems)
	  - ignore xsi:type attributes
      - normalize the whitespaces of all attribute values and text nodes
         leading and trailing whitespaces are removed
         multiple whitespaces are replaced by a single whitespace
      - sort entries in fuelconsumption map and loss-maps (i.e, transmission, axlegear, angledrive)
      - sort entries of torque converter characteristics
      - sort torque limiation entries 
      - sort gears
	  - sort characteristics entries
      - sort axles
	  - sort maxtorquecurve entries
	  - sort powermap entries
      - sort dragcurve entries
	  - sort conditioning entries
	  - sort powermaps by gear attribute
      - sort voltagelevel entries by voltage element value
      - sort ovc entries
      - sort internalresistance entries
      - sort currentlimits entries
-->	<xsl:output omit-xml-declaration="no" indent="yes"/>
	<xsl:template match="*">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*|node()"/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="@xsi:type"/>
	<xsl:template match="@*">
		<xsl:attribute name="{name()}"><xsl:value-of select="normalize-space(.)"/></xsl:attribute>
	</xsl:template>
	<xsl:template match="text()">
         <xsl:value-of select="normalize-space(.)"/>
    </xsl:template>
	<xsl:template match="*[local-name()='FuelConsumptionMap']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@engineSpeed" order="ascending"/>
				<xsl:sort data-type="number" select="@torque" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='FullLoadAndDragCurve']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@engineSpeed" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='TorqueLossMap']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@inputSpeed" order="ascending"/>
				<xsl:sort data-type="number" select="@inputTorque" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='RetarderLossMap']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@retarderSpeed" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='TorqueLimits']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@gear" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='Gears']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@number" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='Characteristics']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@speedRatio" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='Axles']">
		<xsl:apply-templates select="@*"/>
		<xsl:element name="{local-name()}">
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@axleNumber" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	

	<xsl:template match="*[local-name()='MaxTorqueCurve']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@outShaftSpeed" order="ascending"/>
				<xsl:sort data-type="number" select="@maxTorque" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='PowerMap']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@outShaftSpeed" order="ascending"/>
				<xsl:sort data-type="number" select="@torque" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>	
	<xsl:template match="*[local-name()='DragCurve']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@outShaftSpeed" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>		
	<xsl:template match="*[local-name()='Conditioning']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@coolantTempInlet" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>			
	<xsl:template match="*[local-name()='VoltageLevel']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@gear" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>			
	<xsl:template match="*[local-name()='Data']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			
			<xsl:apply-templates select="./*[not(local-name()='DragCurve') and not(local-name()='Conditioning') and not(local-name()='VoltageLevel')]"/>
			
			<xsl:for-each select="*[local-name()='VoltageLevel']">
				<xsl:sort data-type="number" select="*[local-name() = 'Voltage']/text()" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>					
						
			<xsl:for-each select="*[local-name()='DragCurve']">
				<xsl:sort data-type="number" select="@gear" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
			
			<xsl:for-each select="*[local-name()='Conditioning']">
				<xsl:apply-templates select="."/>
			</xsl:for-each>			
		</xsl:element>
	</xsl:template>	
	<xsl:template match="*[local-name()='OCV']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@SoC" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='InternalResistance']">
		<xsl:choose>
			<xsl:when test="*">	
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="@*"/>
					<xsl:for-each select="*">
						<xsl:sort data-type="number" select="@SoC" order="ascending"/>
						<xsl:sort data-type="number" select="@R_2" order="ascending"/>
						<xsl:sort data-type="number" select="@R_10" order="ascending"/>
						<xsl:sort data-type="number" select="@R_20" order="ascending"/>
						<xsl:apply-templates select="."/>
					</xsl:for-each>
				</xsl:element>
			</xsl:when>
			<xsl:otherwise>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="@*|node()"/> 
				</xsl:element>
			</xsl:otherwise>		
		</xsl:choose>
	</xsl:template>		
	<xsl:template match="*[local-name()='CurrentLimits']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@SoC" order="ascending"/>
				<xsl:sort data-type="number" select="@maxChargingCurrent" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	
</xsl:transform>
