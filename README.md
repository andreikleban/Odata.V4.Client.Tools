# dotnet-odata4
[![NuGet](https://img.shields.io/nuget/v/dotnet-odata4.svg)](https://www.nuget.org/packages/dotnet-odata4/)


dotnet-odata4 is a command-line tool for generation OData client proxy classes. It's based on [OData Connected Services Extension](https://github.com/OData/ODataConnectedService).

Installation
------------

The latest stable version is available on [NuGet](https://www.nuget.org/packages/dotnet-odata4/).

```sh
dotnet tool install dotnet-odata4 --global
```


Command-line arguments
------------

| Argument                  | Description |
| -------------             | ----------- |
| **-m**<br>**--metadata**  |The URI of the metadata document. The value must be set to a valid service document URI or a local file path.<br><br>Examples:<br> ```https://services.odata.org/TripPinRESTierService/(S(read))/```<br>```https://services.odata.org/TripPinRESTierService/(S(read))/$metadata```<br>```c:\temp\metadata.xml```<br>```c:\temp\metadata.edmx``` |
|**-n**<br>**--no-tracking**|The use of this disables entity and property tracking|
|**-ns**<br>**--namespace** |The namespace of the client code generated.<br>Namespaces from metadata document are using as a default.|
|**-na**<br>**--no-alias**  |This flag indicates whether to disable naming alias.|
|**-i**<br>**--internal**   |If set to true, generated types will have an "internal" class modifier instead of "public"|
|**-mf**<br>**--multiple**  |This files indicates whether to generate the files into multiple files or single|
|**-eoi**                   |Comma-separated list of the names of operation imports to exclude from the generated code.|
|**-ebo**                   |Comma-separated list of the names of bound operations to exclude from the generated code.|
|**-est**                   |Comma-separated list of the names of entity types to exclude from the generated code.|
|**-c**                     |Custom container class name.|
| **-o**<br>**--outputdir** |Full path to output directory. Current directory is using as a default.|
|**-v**<br>**--verbose**    |Turns on the console output.| 
|**-p**<br>**--proxy**      |Proxy server settings.<br>It needs for access to outside Odata V3 service from private networks.<br>Format: ```domain\user:password@SERVER:PORT```
|**-pl**<br>**--plugins**   |List of postprocessing plugins.<br>Format: ```Assembly.dll,Namespace.Class```|


Examples
------------

OData V4 metadata endpoint and relative output directory:
```
  odata4 -m https://services.odata.org/TripPinRESTierService/(S(read))/$metadata -o ClientDirectory
```
OData V4 service endpoint, relative output directory and no tracking:
```
  odata4 -m https://services.odata.org/TripPinRESTierService/(S(read))/ -o ClientDirectory -n
```
OData V4 service endpoint, relative output directory and multiple files:
```
  odata4 -m https://services.odata.org/TripPinRESTierService/(S(read))/ -o ClientDirectory -mf
```
Metadata document, absolute output directory, output classes namespace and verbocity:
```
  odata4 -m c:\temp\metadata.xml -o c:\temp\OutClientDir -ns Client.Namespace -v
```
OData V4 metadata endpoint and proxy server settings:
```
  odata4 -m https://services.odata.org/TripPinRESTierService/(S(read))/$metadata -p domain\user:userpassword@proxyserver:8080
```
Metadata document and list of postprocessing plugins:
```
  odata4 -m c:\temp\metadata.xml -pl Plugin1Assembly.dll,Namespace.PluginClass1 -pl Plugin2Assembly.dll,Namespace.PluginClass2
```
