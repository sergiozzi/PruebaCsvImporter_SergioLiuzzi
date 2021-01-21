# PruebaCsvImporter - SergioLiuzzi
Prueba de concepto para la lectura de un archivo desde Azure Storage y pasaje de la data a una BD Local de SQL Server

## Consideraciones previas a la primer ejecución :bookmark:

1- Se utiliza como base de datos destino: Server: (localdb)\mssqllocaldb, DataBase: AcmeCorporationDb. Debería crearse manualmente la base de datos y luego ejecutar las migrations del proyecto.

2- En caso que se deseen modificar algunos de los anteriores parámetros, ir al archivo: _Data\CsvImporterDbContext.cs_ para modificarlos.

## Observaciones :pushpin:

1 - Se hizo foco en los tiempos de ejecución para lograr el menor posible, actualmente se debería investigar un mejor modo de bajar el archivo desde el Storage de Azure, ya que por lo que se observa, son archivos que superan los 500MB, en ésta primer versión este punto no está del todo resuelto.

2- Se hace uso de [Entity Framework Extensions] (https://entityframework-extensions.net/) para los procesos de Delete e Insert sobre base de datos, ya que la tabla Stock luego de una importación supera ampliamente el 1M de registros, las pruebas con la librería han mejorado sustancialmente las opciones de EF Core.

3- Restaría agregar componentes de Test para lograr que futuras versiones, permitan automatizar las pruebas de cambios.
