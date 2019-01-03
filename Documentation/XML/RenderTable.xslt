<?xml version="1.0" encoding="UTF-8"?>
<?altova_samplexml ..\..\VectoCore\VectoCore\Resources\XSD\VectoDefinitions.xsd?>
<xsl:stylesheet version="3.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:vecto="urn:tugraz:ivt:VectoAPI:ParameterDocumentation" xmlns:fn="http://www.w3.org/2005/xpath-functions">
	<xsl:output method="html" omit-xml-declaration="yes" encoding="utf-8" indent="yes"/>
	<xsl:param name="pSortingValues" select="'Job,Vehicle,Engine,Gearbox,TorqueConverter,Angulargear,Retarder,Axlegear,Axle,Wheels,Auxiliaries,ADAS,Driver,DrivingCycle'"/>
	<xsl:variable name="vSortingValues" select="concat(',', $pSortingValues, ',')"/>
	<!--

Basic functionality:
  the function 'vecto:GetParameters' recursively traverses all elements, starting from the root and builds a tree according to the xml structure.
  this tree looks as follows:

  (example seee at the end)

	<childnodes>
		<child name="/ElementName/">
			/reference to the actual element in the XSD - xs:element/
			<type>
				/reference to the actual type of this element - xs:complexType or xs:simpleType/
				/contains vectoParameterDefinition/
			</type>
			<childnodes>
				....
			</childnodes>
		</child>
	</childnodes>

	for a correct mapping of the input parameter (element) and the documentation (xs:simpleType) the input parameter has to be annotated with the parameter number
	the xs:simpleType has to contain a vectoParam:description block with all required elements

	this tree is built for engineering mode, declaration mode, and components (declaration mode) separately

 ### Match the whole document  ### 

-->
	<xsl:template match="/">
		<!-- set variables used later -->
		<xsl:variable name="maxParamNumber">
			<xsl:call-template name="maximun">
				<xsl:with-param name="list" select="document(//@schemaLocation)//xs:simpleType//xs:appinfo/vecto:description/vecto:parameterId"/>
			</xsl:call-template>
		</xsl:variable>
		<xsl:variable name="dateTimeNow">
			<xsl:value-of select="current-dateTime()"/>
		</xsl:variable>
		<xsl:variable name="engineeringRoot" select="document(//@schemaLocation)//xs:complexType[@name='VectoJobEngineeringType']"/>
		<xsl:variable name="declarationRoot" select="document(//@schemaLocation)//xs:complexType[@name='VectoDeclarationJobType']"/>
		<xsl:variable name="componentRoot" select="document(//@schemaLocation)//xs:element[@name='VectoInputDeclaration']/xs:complexType"/>
		<xsl:variable name="engineeringParams" select="vecto:GetParameters($engineeringRoot, document(//@schemaLocation))"/>
		<xsl:variable name="declarationParams" select="vecto:GetParameters($declarationRoot, document(//@schemaLocation))"/>
		<xsl:variable name="componentParams" select="vecto:GetParameters($componentRoot, document(//@schemaLocation))"/>
		<!-- HTML output -->
		<xsl:text disable-output-escaping="yes">&lt;!DOCTYPE html&gt;</xsl:text>
		<html>
			<head>
				<title>Vecto Input Parameters</title>
				<script src="https://code.jquery.com/jquery-3.2.1.min.js"/>
				<style type="text/css">
					<xsl:copy-of select="document('style.css')"/>
				</style>
			</head>
			<body>
				<h1>Vecto Input Parameters</h1>
				<div class="schemaVersion">Schema Version: <xsl:value-of select="document(//@schemaLocation)//xs:schema/@version"/>
				</div>
				<div class="generated">Generated: <xsl:value-of select="format-dateTime($dateTimeNow, '[D01].[M01].[Y0001] [H]:[m]')"/>
				</div>
				<div class="nextParameter">Next free parameter: <xsl:value-of select="sum($maxParamNumber + 1)"/>
				</div>
				<table border="1" class="parameters">
					<thead>
						<tr>
							<th class="parameterId">Param ID</th>
							<th class="component">Component</th>
							<th class="name">Name</th>
							<th class="type">Type</th>
							<th class="unit">Unit</th>
							<th class="range">Range</th>
							<th class="genericValues">Generic Values</th>
							<th class="xmlpath">XML Path</th>
							<th class="xmltype">XML Type</th>
							<th class="comment">Definition</th>
						</tr>
					</thead>
					<tbody>
						<xsl:call-template name="RenderTableRows">
							<xsl:with-param name="document" select="document(//@schemaLocation)"/>
							<xsl:with-param name="declarationParams" select="$declarationParams"/>
							<xsl:with-param name="componentParams" select="$componentParams"/>
							<xsl:with-param name="engineeringParams" select="$engineeringParams"/>
						</xsl:call-template>
					</tbody>
				</table>
				<xsl:call-template name="RenderLegend"/>
			</body>
			<script>
				<xsl:value-of select="fn:unparsed-text('custom.js')" disable-output-escaping="yes"/>
			</script>
		</html>
	</xsl:template>
	<!--

 #### END WHOLE DOCUMENT ### 

-->
	<xsl:template name="RenderTableRows">
		<xsl:param name="document"/>
		<xsl:param name="declarationParams"/>
		<xsl:param name="componentParams"/>
		<xsl:param name="engineeringParams"/>
		<xsl:for-each select="Q{http://www.altova.com/xslt-extensions}distinct-nodes(($declarationParams|$engineeringParams)//xs:appinfo/vecto:description/vecto:parameterId[contains(ancestor-or-self::child[1]/(xs:element|xs:attribute)/xs:annotation/xs:documentation, text())])">
			<xsl:sort data-type="number" select="string-length(substring-before($vSortingValues,concat(',',@component,',')))"/>
			<xsl:sort data-type="number" select="text()"/>
			<xsl:call-template name="RenderParam">
				<xsl:with-param name="mergedDocuments" select="$document"/>
				<xsl:with-param name="declarationParams" select="$declarationParams"/>
				<xsl:with-param name="componentParams" select="$componentParams"/>
				<xsl:with-param name="engineeringParams" select="$engineeringParams"/>
			</xsl:call-template>
		</xsl:for-each>
	</xsl:template>
	<!--

=========================
-->
	<xsl:template name="RenderParam">
		<xsl:param name="mergedDocuments"/>
		<xsl:param name="declarationParams"/>
		<xsl:param name="componentParams"/>
		<xsl:param name="engineeringParams"/>
		<xsl:for-each select=".">
			<xsl:apply-templates select=".">
				<xsl:with-param name="mergedDocuments" select="$mergedDocuments"/>
				<xsl:with-param name="declarationParams" select="$declarationParams"/>
				<xsl:with-param name="componentParams" select="$componentParams"/>
				<xsl:with-param name="engineeringParams" select="$engineeringParams"/>
			</xsl:apply-templates>
		</xsl:for-each>
	</xsl:template>
	<!--

=========================
-->
	<xsl:template match="vecto:parameterId">
		<xsl:param name="mergedDocuments"/>
		<xsl:param name="declarationParams"/>
		<xsl:param name="componentParams"/>
		<xsl:param name="engineeringParams"/>
		<xsl:param name="xmlTreeNode"/>
		<xsl:variable name="paramTypeName" select="./ancestor::xs:simpleType/@name"/>
		<xsl:variable name="paramValue" select="text()"/>
		
				<xsl:call-template name="RenderSingleRow">
					<xsl:with-param name="mergedDocuments" select="$mergedDocuments"/>
					<xsl:with-param name="declarationParams" select="$declarationParams"/>
					<xsl:with-param name="componentParams" select="$componentParams"/>
					<xsl:with-param name="engineeringParams" select="$engineeringParams"/>
				</xsl:call-template>
			
	</xsl:template>
	<!--

=========================
-->
	<xsl:template name="RenderSingleRow">
		<xsl:param name="mergedDocuments"/>
		<xsl:param name="declarationParams"/>
		<xsl:param name="componentParams"/>
		<xsl:param name="engineeringParams"/>
		<xsl:variable name="paramNbr" select="./text()"/>
		<xsl:variable name="declarationInstance" select="$declarationParams//xs:appinfo/vecto:description/vecto:parameterId[contains(ancestor-or-self::child[1]/(xs:element|xs:attribute)/xs:annotation/xs:documentation, text())][text()=$paramNbr]"/>
		<xsl:variable name="componentInstance" select="$componentParams//xs:appinfo/vecto:description/vecto:parameterId[contains(ancestor-or-self::child[1]/(xs:element|xs:attribute)/xs:annotation/xs:documentation, text())][text()=$paramNbr]"/>
		<xsl:variable name="engineeringInstance" select="$engineeringParams//xs:appinfo/vecto:description/vecto:parameterId[contains(ancestor-or-self::child[1]/(xs:element|xs:attribute)/xs:annotation/xs:documentation, text())][text()=$paramNbr]"/>
		<tr>
			<xsl:variable name="cssClass" select="../@status | vecto:ContainsParameter($declarationInstance, 'Declaration ') | vecto:ContainsParameter($engineeringInstance, 'Engineering ')"/>
			<xsl:if test="$cssClass">
				<xsl:attribute name="class"><xsl:value-of select="$cssClass" separator=" "/></xsl:attribute>
			</xsl:if>
			<td class="parameterId">
				<xsl:value-of select="format-number(., 'P000')"/>
			</td>
			<td class="component">
				<xsl:value-of select="@component"/>
			</td>
			<td class="name">
				<xsl:variable name="elementName" select="fn:distinct-values(($declarationInstance|$engineeringInstance)/ancestor::child[1]/@nodeName)"/>
				<xsl:choose>
					<xsl:when test="count($elementName) eq 1">
						<xsl:value-of select="$elementName"/>
					</xsl:when>
					<xsl:otherwise>
						<div class="declaration">
							<xsl:value-of select="$declarationInstance/ancestor::child[1]/@nodeName"/>
						</div>
						<div class="engineering">
							<xsl:value-of select="$engineeringInstance/ancestor::child[1]/@nodeName"/>
						</div>
					</xsl:otherwise>
				</xsl:choose>
				<!--xsl:if test="not($xsdInstance/@name)">
					<xsl:value-of select="./@usage"/>
				</xsl:if-->
			</td>
			<!--<td class="description"/>-->
			<td class="type">
				<xsl:value-of select="substring-after(./ancestor::xs:simpleType/xs:restriction/@base, ':')"/>
			</td>
			<td class="unit">[<xsl:value-of select="../vecto:unit"/>]</td>
			<xsl:call-template name="ParameterRange"/>
			<td class="genericValues">
				<span>
					<xsl:value-of select="../vecto:genericValueType"/>
				</span>
				<xsl:if test="../vecto:genericValueSource"> (<span>
						<xsl:value-of select="../vecto:genericValueSource"/>)</span>
				</xsl:if>
			</td>
			<td class="xmlpath">
				<xsl:attribute name="sorting"><xsl:call-template name="RenderXPathOrder"><xsl:with-param name="xpath" select="vecto:GetXMLPath($declarationInstance/ancestor::child[1]/*[1])"/></xsl:call-template></xsl:attribute>
				<xsl:if test="$declarationInstance/ancestor::child[1]/*[1]">
				<div class="declaration">
					<span class="element">VectoInputDeclaration</span>
					<xsl:call-template name="RenderXPath">
						<xsl:with-param name="xpath" select="vecto:GetXMLPath($declarationInstance/ancestor::child[1]/*[1])"/>
					</xsl:call-template>
				</div>
				</xsl:if>
				<xsl:if test="$componentInstance/ancestor::child[1]/*[1]">
				<div class="declaration component">
					<span class="element">VectoInputDeclaration</span>
					<xsl:call-template name="RenderXPath">
						<xsl:with-param name="xpath" select="vecto:GetXMLPath($componentInstance/ancestor::child[1]/*[1])"/>
					</xsl:call-template>
				</div>
				</xsl:if>
				<div class="engineering">
					<xsl:for-each select="$engineeringInstance">
						<div>
							<span class="element">
								<xsl:choose>
									<xsl:when test="./ancestor::child[xs:element/@name='Vehicle']">VectoInputEngineering</xsl:when>
									<xsl:otherwise>VectoComponentEngineering</xsl:otherwise>
								</xsl:choose>
							</span>
							<xsl:call-template name="RenderXPath">
								<xsl:with-param name="xpath" select="vecto:GetXMLPath(./ancestor::child[1]/*[1])"/>
							</xsl:call-template>
						</div>
					</xsl:for-each>
				</div>
			</td>
			<td class="xmltype">
				<xsl:value-of select="$declarationInstance/ancestor::child[1]/*[1]/@type"/>
			</td>
			<td class="comment">
				<xsl:if test="@status = 'deprecated'">DEPRECATED<br/>
				</xsl:if>
				<xsl:call-template name="GetEnumerationValues">
					<xsl:with-param name="declarationInstance" select="$declarationInstance"/>
					<xsl:with-param name="engineeringInstance" select="$engineeringInstance"/>
				</xsl:call-template>
				<xsl:value-of select="../vecto:comment"/>
				<!--<xsl:if test="$xsdInstance[@minOccurs=0]"><span class="optional">optional</span></xsl:if>-->
				<xsl:if test="($declarationInstance)/ancestor::child[1]/(xs:element|xs:attribute)[@minOccurs=0]">
					<span class="optionalDeclaration">optional in Declaration mode</span>
				</xsl:if>
				<xsl:if test="($engineeringInstance)/ancestor::child[1]/(xs:element|xs:attribute)[@minOccurs=0]">
					<span class="optionalEngineering">optional in Engineering mode</span>
				</xsl:if>
				
			</td>
		</tr>
	</xsl:template>
	<!--

=========================
-->
	<xsl:template name="GetEnumerationValues">
		<xsl:param name="declarationInstance"/>
		<xsl:param name="engineeringInstance"/>
		<xsl:if test="count(($declarationInstance|$engineeringInstance)/ancestor::child[1]/type//xs:restriction/xs:enumeration) gt 0">
			<div class="declaration">
				<xsl:if test="count(($declarationInstance)/ancestor::child[1]/type//xs:restriction/xs:enumeration) gt 0">
					Allowed values: <xsl:for-each select="$declarationInstance/ancestor::child[1]/type//xs:restriction/xs:enumeration">
						<span class="enumEntry">&quot;<xsl:value-of select="@value"/>&quot;</span>
						<xsl:if test="position() != last()">, </xsl:if>
					</xsl:for-each>
				</xsl:if>
			</div>
			<div class="engineering">
				<xsl:if test="count(($engineeringInstance)/ancestor::child[1]/type//xs:restriction/xs:enumeration) gt 0">
					Allowed values: <xsl:for-each select="$engineeringInstance/ancestor::child[1]/type//xs:restriction/xs:enumeration">
						<span class="enumEntry">&quot;<xsl:value-of select="@value"/>&quot;</span>
						<xsl:if test="position() != last()">, </xsl:if>
					</xsl:for-each>
				</xsl:if>
			</div>
			<br/>
		</xsl:if>
	</xsl:template>
	<!--

=========================
-->
	<xsl:template name="ParameterRange">
		<xsl:variable name="restriction" select="../../../../xs:restriction[@base='xs:double' or @base='xs:int' or fn:starts-with(@base, 'tns:Double')]"/>
		<td class="range">
			<xsl:choose>
				<xsl:when test="boolean($restriction/xs:minInclusive/@value)">
				&#8805; <xsl:value-of select="$restriction/xs:minInclusive/@value"/>
					<xsl:call-template name="RenderUnit"/>
				</xsl:when>
				<xsl:when test="boolean($restriction/xs:minExclusive/@value)">
				&gt; <xsl:value-of select="$restriction/xs:minExclusive/@value"/>
					<xsl:call-template name="RenderUnit"/>
				</xsl:when>
				<xsl:otherwise/>
			</xsl:choose>
			<xsl:choose>
				<xsl:when test="boolean($restriction/xs:maxInclusive/@value)">
				&#8804; <xsl:value-of select="$restriction/xs:maxInclusive/@value"/>
					<xsl:call-template name="RenderUnit"/>
				</xsl:when>
				<xsl:when test="boolean($restriction/xs:maxExclusive/@value)">
				&lt; <xsl:value-of select="$restriction/xs:maxExclusive/@value"/>
					<xsl:call-template name="RenderUnit"/>
				</xsl:when>
				<xsl:otherwise/>
			</xsl:choose>
		</td>
	</xsl:template>
	<!--

=========================
-->
	<xsl:template name="RenderUnit">
		<xsl:if test="not(../vecto:unit = '-')">
			<span class="unit">
				<xsl:value-of select="../vecto:unit"/>
			</span>
		</xsl:if>
	</xsl:template>
	<xsl:template name="maximun">
		<xsl:param name="list"/>
		<xsl:for-each select="$list">
			<xsl:sort select="." data-type="number" order="descending"/>
			<xsl:if test="position()=1">
				<xsl:value-of select="."/>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>
	<xsl:template name="RenderLegend">
		<div class="legend">
			<div class="Declaration">
				<span class="marker"/> Used only in Declaration mode</div>
			<div class="Declaration Engineering">
				<span class="marker"/> Used in Declaration and Engineering mode</div>
			<div class="Engineering">
				<span class="marker"/> Used only in Engineering Mode</div>
		</div>
	</xsl:template>
	<xsl:template name="RenderXPath">
		<xsl:param name="xpath"/>
		<xsl:for-each select="$xpath">
			<span>
				<xsl:attribute name="class"><xsl:value-of select="@type"/></xsl:attribute>
				<xsl:value-of select="text()"/>
			</span>
		</xsl:for-each>
	</xsl:template>
	<xsl:template name="RenderXPathOrder">
		<xsl:param name="xpath"/>
		<xsl:for-each select="$xpath">
			<xsl:value-of select="format-number(./@position, '00')"/>#</xsl:for-each>
	</xsl:template>
	<!--
		###### FUNCTIONS #######
 -->
	<xsl:function name="vecto:GetParameters">
		<xsl:param name="rootNode"/>
		<xsl:param name="mergedDocuments"/>
		<xsl:variable name="nodes" select="vecto:GetContainingElements($rootNode,$mergedDocuments)"/>
		<xsl:element name="childNodes">
			<xsl:for-each select="$nodes">
				<xsl:element name="child">
					<xsl:attribute name="nodeName" select="@name"/>
					<xsl:variable name="childType" select="vecto:GetTypeForElement(., $mergedDocuments)"/>
					<xsl:sequence select="."/>
					<xsl:element name="type">
						<xsl:sequence select="$childType"/>
					</xsl:element>
					<xsl:if test="$childType">
						<xsl:sequence select="vecto:GetParameters($childType, $mergedDocuments)"/>
					</xsl:if>
				</xsl:element>
			</xsl:for-each>
			<!--<xsl:if test="$childTypes">
				<xsl:sequence select="vecto:GetParameters( $childTypes, $mergedDocuments )"/>
			</xsl:if>-->
		</xsl:element>
		<!--<xsl:element name="types">
			<xsl:sequence select="$mergedDocuments//xs:simpleType[@name=$nodes/@type]"/>
		</xsl:element>-->
	</xsl:function>
	<!--

-->
	<xsl:function name="vecto:GetContainingElements">
		<xsl:param name="node"/>
		<xsl:param name="mergedDocuments"/>
		<xsl:choose>
			<xsl:when test="$node">
				<xsl:variable name="childElementsAttributes" select="$node/(xs:element|xs:attribute)"/>
				<xsl:variable name="nestedChildElemens" select="vecto:GetContainingElements($node//(xs:sequence|xs:choice), $mergedDocuments)"/>
				<xsl:variable name="derivedElements" select="vecto:GetComplexContent($node/xs:complexContent|$node/xs:complexType/xs:complexContent, $mergedDocuments)"/>
				<xsl:sequence select="$childElementsAttributes|$nestedChildElemens|$derivedElements"/>
			</xsl:when>
		</xsl:choose>
	</xsl:function>
	<!--

-->
	<xsl:function name="vecto:GetComplexContent">
		<xsl:param name="complexContent"/>
		<xsl:param name="mergedDocuments"/>
		<xsl:if test="$complexContent">
			<xsl:variable name="baseTypeName" select="replace($complexContent/xs:extension/@base,  '([a-z]*):(.*)', '$2')"/>
			<xsl:variable name="baseType" select="$mergedDocuments//(xs:complexType|xs:simpleType)[@name=$baseTypeName]"/>
			<xsl:variable name="extensions" select="$complexContent/xs:extension/(xs:attribute|(xs:sequence/(xs:element|xs:attribute)))"/>
			<xsl:sequence select="vecto:GetContainingElements($baseType, $mergedDocuments)|$extensions"/>
		</xsl:if>
	</xsl:function>
	<!--

-->
	<xsl:function name="vecto:GetTypeForElement">
		<xsl:param name="element"/>
		<xsl:param name="mergedDocuments"/>
		<xsl:choose>
			<xsl:when test="$element">
				<xsl:variable name="typeNames" select="$element[@type]/@type"/>
				<xsl:variable name="anonymousTypes" select="$element/(xs:complexType|xs:simpleType)"/>
				<xsl:variable name="namedTypes" select="$mergedDocuments//(xs:complexType|xs:simpleType)[@name=replace($typeNames, '([a-z]*):(.*)', '$2')]"/>
				<xsl:sequence select="$namedTypes | $anonymousTypes"/>
			</xsl:when>
		</xsl:choose>
	</xsl:function>
	<!--

-->
	<xsl:function name="vecto:GetXMLPath">
		<xsl:param name="node"/>
		<xsl:if test="$node">
			<xsl:variable name="prefix" select="vecto:GetXPathPrefix($node)"/>
			<xsl:variable name="position" select="$node/ancestor-or-self::child[1]/fn:position()"/>
			<xsl:sequence select="vecto:GetXMLPath($node/ancestor-or-self::child[1]/../../node())"/>
			<xsl:element name="xPathEntry">
				<xsl:attribute name="type" select="$prefix"/>
				<xsl:attribute name="position" select="count($node/ancestor-or-self::child[1]/preceding-sibling::*)"/>
				<xsl:value-of select="$node/ancestor-or-self::child[1]/@nodeName"/>
			</xsl:element>
		</xsl:if>
	</xsl:function>
	<xsl:function name="vecto:GetXPathPrefix">
		<xsl:param name="node"/>
		<xsl:choose>
			<xsl:when test="$node[1]/name() = 'xs:attribute'">attribute</xsl:when>
			<xsl:when test="$node[1]/name() = 'xs:element'">element</xsl:when>
			<xsl:otherwise>unknown</xsl:otherwise>
		</xsl:choose>
	</xsl:function>
	<xsl:function name="vecto:ContainsParameter">
		<xsl:param name="instance"/>
		<xsl:param name="output"/>
		<xsl:if test="fn:count($instance)">
			<xsl:value-of select="$output"/>
		</xsl:if>
	</xsl:function>
	<xsl:function name="vecto:SearchParameter">
		<xsl:param name="instance"/>
		<xsl:param name="parameterSet"/>
		<xsl:sequence select="$parameterSet//xs:simpleType//vecto:description[vecto:parameterId=$instance]"/>
	</xsl:function>
</xsl:stylesheet>

<!--

#################################

<?xml version="1.0" encoding="UTF-8"?>
<childNodes>
	<child nodeName="Vehicle">
		<xs:element name="Vehicle">
			<xs:complexType>
				<xs:complexContent>
					<xs:extension base="tns:VehicleDeclarationType"/>
				</xs:complexContent>
			</xs:complexType>
		</xs:element>
		<type>
			<xs:complexType>
				<xs:complexContent>
					<xs:extension base="tns:VehicleDeclarationType"/>
				</xs:complexContent>
			</xs:complexType>
		</type>
		<childNodes>
			<child nodeName="Manufacturer">
				<xs:element name="Manufacturer" type="tns:ManufacturerType">
					<xs:annotation>
						<xs:documentation>P235</xs:documentation>
					</xs:annotation>
				</xs:element>
				<type>
					[...]
				</type>
				<childNodes/>
			</child>
			<child nodeName="ManufacturerAddress">
				[...]
			</child>
			<child nodeName="VehicleCategory">
				[...]
			</child>
			<child nodeName="AxleConfiguration">
				[...]
			</child>
			[...]
			<child nodeName="Components">
				<xs:element name="Components">
					[...]
				</xs:element>
				<type>
					[...]
				</type>
				<childNodes>
					<child nodeName="Engine">
						<xs:element name="Engine" type="tns:EngineComponentDeclarationType"/>
						<type>
							[...]
						</type>
						<childNodes>
							<child nodeName="Data">
								<xs:element name="Data" type="tns:EngineDataDeclarationType"/>
								<type>
									[...]
								</type>
								<childNodes>
									<child nodeName="Displacement">
										[...]
									</child>
									<child nodeName="IdlingSpeed">
										<xs:element name="IdlingSpeed" type="tns:EngineDeclaredSpeedType">
											<xs:annotation>
												<xs:documentation>P063 - [1/min]</xs:documentation>
											</xs:annotation>
										</xs:element>
										<type>
											<xs:simpleType name="EngineDeclaredSpeedType">
												<xs:annotation>
													<xs:appinfo>
														<vectoParam:description>
															<vectoParam:parameterId component="Engine">063</vectoParam:parameterId>
															<vectoParam:parameterId component="Vehicle">198</vectoParam:parameterId>
															<vectoParam:parameterId component="Engine">249</vectoParam:parameterId>
															<vectoParam:unit>1/min</vectoParam:unit>
														</vectoParam:description>
													</xs:appinfo>
													<xs:documentation>P063, P249 - [1/min]</xs:documentation>
												</xs:annotation>
												<xs:restriction base="xs:int">
													<xs:minInclusive value="400"/>
													<xs:maxInclusive value="10000"/>
												</xs:restriction>
											</xs:simpleType>
										</type>
										<childNodes/>
									</child>
									<child nodeName="RatedSpeed">
										<xs:element name="RatedSpeed" type="tns:EngineDeclaredSpeedType">
											<xs:annotation>
												<xs:documentation>P249 - [1/min]</xs:documentation>
											</xs:annotation>
										</xs:element>
										<type>
											<xs:simpleType name="EngineDeclaredSpeedType">
												<xs:annotation>
													<xs:appinfo>
														<vectoParam:description>
															<vectoParam:parameterId component="Engine">063</vectoParam:parameterId>
															<vectoParam:parameterId component="Vehicle">198</vectoParam:parameterId>
															<vectoParam:parameterId component="Engine">249</vectoParam:parameterId>
															<vectoParam:unit>1/min</vectoParam:unit>
														</vectoParam:description>
													</xs:appinfo>
													<xs:documentation>P063, P249 - [1/min]</xs:documentation>
												</xs:annotation>
												<xs:restriction base="xs:int">
													<xs:minInclusive value="400"/>
													<xs:maxInclusive value="10000"/>
												</xs:restriction>
											</xs:simpleType>
										</type>
										<childNodes/>
									</child>

								</childNodes>
							</child>
							<child nodeName="Signature">
								[...]
							</child>
						</childNodes>
					</child>
					<child nodeName="Gearbox">
						[...]
					</child>

				</childNodes>
			</child>
		</childNodes>
	</child>

</childNodes>

-->