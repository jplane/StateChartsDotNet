
Remove-Item -Force -Recurse -ErrorAction Ignore "..\..\WebHost\bin"
Remove-Item -Force -Recurse -ErrorAction Ignore "..\..\WebHost\obj"
Remove-Item -Force -Recurse -ErrorAction Ignore ".\src"

Copy-Item -Force -Recurse "..\metadata\" -Destination "src\function-defs\metadata"
Copy-Item -Force -Recurse "..\register\" -Destination "src\function-defs\register"
Copy-Item -Force -Recurse "..\registerandstart\" -Destination "src\function-defs\registerandstart"
Copy-Item -Force -Recurse "..\start\" -Destination "src\function-defs\start"
Copy-Item -Force -Recurse "..\sendmessage\" -Destination "src\function-defs\sendmessage"
Copy-Item -Force -Recurse "..\status\" -Destination "src\function-defs\status"
Copy-Item -Force -Recurse "..\stop\" -Destination "src\function-defs\stop"

Copy-Item -Force -Recurse "..\host.json" -Destination "src\host.json"
Copy-Item -Force -Recurse ".\proxies.json" -Destination "src\proxies.json"
Copy-Item -Force -Recurse ".\publish.sh" -Destination "src\publish.sh"

Copy-Item -Force -Recurse "..\..\WebHost\" -Destination "src\app\WebHost"
Copy-Item -Force -Recurse "..\..\Common\" -Destination "src\app\Common"
Copy-Item -Force -Recurse "..\..\Engine\" -Destination "src\app\Engine"
Copy-Item -Force -Recurse "..\..\DurableEngine\" -Destination "src\app\DurableEngine"
Copy-Item -Force -Recurse "..\..\Metadata.Fluent\" -Destination "src\app\Metadata.Fluent"
Copy-Item -Force -Recurse "..\..\Metadata.Xml\" -Destination "src\app\Metadata.Xml"
Copy-Item -Force -Recurse "..\..\Metadata.Json\" -Destination "src\app\Metadata.Json"

docker build -t scdn/azure-deploy:v1 .

docker run -it scdn/azure-deploy:v1