# VECTO Hashing Tool

Version: 1.2

The VECTO Hashing tool provides functionality for hashing coponent data, checking the integrity of component data, checking the integrity of VECTO job data, and checking the integrity of VECTO reports and its job files.

## General 

VECTO input data and VECTO results for certification of heavy duty vehicles uses the XML format. The XML schema for
botht, input data and results, can be found under the following URL and are distributed with the VECTO simulation tool.

- https://webgate.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/XSD/



## Method of Hash Computation

### Introduction

The integrity of electronic data, i.e., component data, job data, and VECTO results is of major importance throughout the whole process of vehicle CO2 certification and in particular among data exchange between the involved participants. The Commission and industry partners agreed to use (witnessed) cryptographic hashes as 
integrity measure for all kinds of data. The digest value (cryptographic hash) shall be stored at a second 
site, i.e., the type approval authority in case of component data, and the CO2 monitoring instance and the customer in case of the VECTO simulation results. Comparing the digest value of a component in the job data or the simulation result data with the digest value stored at the type approval authority allows to confirm the integrity of the component data, for example.

VECTO component data, VECTO job data, and VECTO simulation results are handled in XML format. Consequently, the agreed method for computing the digest value of electronic data is based on the [XML Dsig|https://www.w3.org/TR/xmldsig-core/] standard, which is also used for [eIDAS|http://data.europa.eu/eli/reg/2014/910/oj] and [XML Advanced Electronic Signatures(XAdES)|https://www.w3.org/TR/XAdES/].

For VECTO related data the detached signature approach is used, where the component data and the signature element are in the same XML document.

The XML representation of a certain XML document is ambiguous. Whitespaces, line breaks, comments, etc. may be added in various positions without actually altering the XML document's data. This is a huge drawback when directly applying cryptographic methods on XML documents, because changing for example the indentation (tab vs. spaces) invalidates all cryptographic operations although the content (and semantic) of the XML data is still the same. Therefore, it is crucial to use the same physical representation (called canonical form) of an XML document before applying cryptographic operations.
Consequently, if two documents have the same canonical form, then the two documents are logically equivalent 
within the given application context. The [Canonical XML|https://www.w3.org/TR/xml-c14n11/] standard defines general transformations applied to an XML document to derive its canonical form. However, even after applying the canonicalization as described in the canonicalization standard, two XML documents with different canonical forms may still be equivalent in the context of VECTO for the following reasons:

  - Entries in loss-maps, engine full-load curve, engine fuel-consumption map, etc. may be listed in 
    arbitrary order
  - Gear entries in the transmission component may be listed in arbitrary order
  - The vehicle's axles may be listed in arbitrary order
  - Numeric values may be provided in different accuracy without affecting the simulation results.

For the last issue, the XML schema has been designed to require a defined number of digits after the decimal sign and no leading zeros are allowed. 

To cope with the first three issues, an additional canonicalization transformation of the XML document is necessary. This canonicalization transformation sorts all ambiguous entries in a defined manner and is described programming language independent as XSLT transformaton. This XSLT transformation can be found under the following address and is shipped together with the VECTO hashing tool.

https://webgate.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/HashingXSLT/

### Hash Computation

Computing the digest value of an XML document in the VECTO context is done applying the following steps:

  1. Apply the VECTO-specific canonicalization (i.e., sorting of ambiguous entries). This transformation is 
     identified via the URI "urn:vecto:xml:2017:canonicalization" 
  2. Apply the generic XML canonicalization (http://www.w3.org/2001/10/xml-exc-c14n#)
  3. Compute the digest value using one of the supported digest methods

Currently, only two canonicalization methods, namely http://www.w3.org/2001/10/xml-exc-c14n#, and urn:vecto:xml:2017:canonicalization, and one digest method, namely http://www.w3.org/2001/04/xmlenc#sha256 are supported. Both canonicalization methods are mandatory (as described above). Further methods may be added later.

## Hashing Component Data

The "Hash Component Data" window allows to compute the digest value and adds the Signature element to an XML component file.

The selected component data file has to contain the structure of an XML component file as described in the XML schema, *except* it *must not* contain the Signature element following the Data element. Moreover, the selected file has to be a component file (engine, gearbox, axlegear, angledrive, tyre, retarder, torque converter), other file types such as job files and report files are *not supported*!.

The component data may already contain an id-attribute in the Data element. However, if the length of the id value is less than 5 characters it will be overwritten by the hashing tool in order to guarantee sufficient uniqueness.

The Date element in the component data will be overwritten in any case with the current time.

As a refeence for the user, the GUI shows the canonicalization method and digest method used as well as the computed digest value. The digest value can be easily copied to be used in other applications or filled into the certification report.

If the generated component file validates against the XML schema it can be saved to disk. Otherwise, the error messges and warnings can be inspected via the /Details.../ button.

## Verifying Integrity Component Data


## Verifying Integrity VECTO Job Data


## Verifying Integrity VECTO Results


## Using the Hashing Library