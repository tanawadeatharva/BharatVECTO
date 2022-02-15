
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
	<xsl:template name="DragCurveTemplate" match="*[local-name()='DragCurve']">
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
				<xsl:sort data-type="number" select="@coolingPower" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>

	<xsl:template name="VoltageLevelTemplate" match="*[local-name()='VoltageLevel']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>	
			<xsl:apply-templates select="*[not(local-name()='PowerMap')]"/> 	
			<xsl:apply-templates select="*[local-name()='PowerMap']"> 
				<xsl:sort select="@gear" order="ascending" data-type="number" />			
			</xsl:apply-templates>	
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="*[local-name()='PowerMap']" >
		<xsl:element name="{local-name()}">
		  <xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@outShaftSpeed" order="ascending"/>
				<xsl:sort data-type="number" select="@torque" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>

	<xsl:template match="*[local-name()='VoltageLevel' and ./*[local-name() = 'Voltage']]">		
		<xsl:if test="count(preceding-sibling::*[local-name()='VoltageLevel']) > (count(//*[local-name()='VoltageLevel']) - 2)">
			<xsl:for-each select="../*[local-name()='VoltageLevel']">
				<xsl:sort data-type="number" select="./*[local-name() = 'Voltage']/text()" order="ascending"/>			
				<xsl:call-template name="VoltageLevelTemplate"/>
			</xsl:for-each>			
		</xsl:if>				
	</xsl:template>

	<xsl:template match="*[local-name()='DragCurve' and @gear]">
		<xsl:if test="count(preceding-sibling::*[local-name()='DragCurve']) > (count(//*[local-name()='DragCurve']) - 2)">	
			<xsl:for-each select="../*[local-name()='DragCurve']">
				<xsl:sort data-type="number" select="@gear" order="ascending"/>
				<xsl:call-template name="DragCurveTemplate"/>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>


	
	<!--

	<xsl:for-each select="../*[local-name()='VoltageLevel' and ./*[local-name() = 'Voltage']]">
		<xsl:sort data-type="number" select="./*[local-name() = 'Voltage']/text()" order="ascending"/>					
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*|node()"/> 
		</xsl:element>
	</xsl:for-each>

	
	<xsl:template match="*[local-name()='Data']">
		<xsl:choose>		
		
			<xsl:when test="*[local-name()='VoltageLevel' and ./*[local-name() = 'Voltage']]/*[local-name()='Voltage' ]
						and *[local-name()='DragCurve']/@gear 
						and *[local-name() ='Conditioning' ]">
			
				<xsl:apply-templates select="@*"/>
				
				<xsl:apply-templates select="./*[not(local-name()='DragCurve') 
				and not(local-name()='Conditioning') 
				and not(local-name()='VoltageLevel')]"/>
		
				<xsl:apply-templates select="*[local-name()='VoltageLevel']"> 
					<xsl:sort data-type="number" select="*[local-name() = 'Voltage']/text()" order="ascending"/>
				</xsl:apply-templates>
				
				<xsl:apply-templates select="*[local-name()='DragCurve']">
					<xsl:sort data-type="number" select="@gear" order="ascending"/>
				</xsl:apply-templates>				
				
				<xsl:apply-templates select="*[local-name()='Conditioning']"/> 	
								
			</xsl:when>		
					
			<xsl:when test="*[local-name()='VoltageLevel']/*[local-name()='Voltage'] 
						and *[local-name()='DragCurve'][not(@gear)]
						and *[local-name()='Conditioning']"> 
			
				<xsl:apply-templates select="@*"/>
				
				<xsl:apply-templates select="./*[not(local-name()='DragCurve') 
				and not(local-name()='Conditioning') 
				and not(local-name()='VoltageLevel')]"/>
		
				<xsl:apply-templates select="*[local-name()='VoltageLevel']"> 
					<xsl:sort data-type="number" select="*[local-name() = 'Voltage']/text()" order="ascending"/>
				</xsl:apply-templates>
				
				<xsl:apply-templates select="*[local-name()='DragCurve']"/>
				<xsl:apply-templates select="*[local-name()='Conditioning']"/> 					
			</xsl:when>
			
			<xsl:when test="*[local-name()='VoltageLevel']/*[not(local-name()='Voltage')] 
						and *[local-name()='DragCurve'][not(@gear)] 
						and *[not(local-name()='Conditioning')]">
			
				<xsl:apply-templates select="@*"/>
				
				<xsl:apply-templates select="./*[not(local-name()='DragCurve') 
				and not(local-name()='VoltageLevel')]"/>
		
				<xsl:apply-templates select="*[local-name()='VoltageLevel']"> 
					<xsl:sort data-type="number" select="*[local-name() = 'Voltage']/text()" order="ascending"/>
				</xsl:apply-templates>
				
				<xsl:apply-templates select="*[local-name()='DragCurve']"/>
			</xsl:when>
			
			<xsl:otherwise>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="@*|node()"/> 
				</xsl:element>
			</xsl:otherwise>		
		</xsl:choose>
	</xsl:template>
	
-->



	<!--
	
		<xsl:element name="{local-name()}">			

			<xsl:for-each select="./*[local-name()='VoltageLevel']">
				<xsl:sort data-type="number" select="*[local-name() = 'Voltage']/text()" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
			
			
						<xsl:apply-templates select="@*"/>	
			<xsl:apply-templates select="*[not(local-name()='VoltageLevel')]"/> 			
		</xsl:element>
		-->



	
<!--




	<xsl:template match="*[local-name()='Data']">	
	
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:apply-templates select="*[not(local-name()='VoltageLevel')]"/>
			<xsl:apply-templates select="*[local-name()='VoltageLevel']"> 
				<xsl:sort data-type="number" select="*[local-name() = 'Voltage']/text()" order="ascending"/>
	
			</xsl:apply-templates>	
		</xsl:element>
	</xsl:template>



<xsl:template match="*" mode="copy-no-namespaces">
    <xsl:element name="{local-name()}">
        <xsl:copy-of select="@*"/>
        <xsl:apply-templates select="node()" mode="copy-no-namespaces"/>
    </xsl:element>
</xsl:template>

<xsl:template match="comment()| processing-instruction()" mode="copy-no-namespaces">
    <xsl:copy/>
</xsl:template>





<xsl:template match="*[local-name()='PowerMap']" mode="copy-no-namespaces">
  <xsl:copy-of select="." />

</xsl:template> -->



<!--

		<xsl:template match="*[local-name()='VoltageLevel']">
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="@*"/>
					<xsl:for-each select="*">
						<xsl:sort data-type="number" select="@gear" order="ascending"/>
						<xsl:apply-templates select="."/>
					</xsl:for-each>
				</xsl:element>
		</xsl:template>

-->
	<!---
	<xsl:template match="*[local-name()='Data']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			
			<xsl:apply-templates select="./*[not(local-name()='DragCurve') 
			and not(local-name()='Conditioning') 
			and not(local-name()='VoltageLevel')
			and not(local-name()='Mode')]"/>
	
			<xsl:for-each select="*[local-name()='Mode']">
				<xsl:sort data-type="number" select="*[local-name() = 'IdlingSpeed']/text()" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>		
			
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
				-->
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
	
<!-- 	<xsl:template match="*[local-name()='Mode']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="text" select="@type" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template> -->
	
</xsl:transform>
