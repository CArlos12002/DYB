### Documentación y Guía para Ejecutar la API `XmlDocs` Usando Swagger y Visual Studio

#### **Descripción del Proyecto**
Este proyecto es una API desarrollada en ASP.NET Core que permite gestionar documentos XML. La API se conecta a una base de datos Redis para almacenar y recuperar información de los documentos, y también almacena los documentos en el sistema de archivos local. La API ofrece dos endpoints principales: uno para recuperar documentos y otro para guardarlos.

#### **Endpoints Disponibles**

1. **GET `/api/xml-docs/document/{documentId}`**
   - **Descripción**: Recupera un documento por su ID.
   - **Parámetros**:
     - `documentId` (string): El ID del documento.
   - **Respuestas**:
     - **200 OK**: Documento recuperado con éxito.
     - **404 Not Found**: Documento no encontrado.
     - **410 Gone**: Documento no disponible, pero se devuelve la información del documento incluyendo la ruta completa de almacenamiento.
     - **422 Unprocessable Entity**: Error de validación.

2. **PUT `/api/xml-docs/document/{businessModel}/{year}/{month}/{dayOfMonth}/{hour}/{guid}`**
   - **Descripción**: Guarda un documento XML.
   - **Parámetros**:
     - `businessModel` (string): Modelo de negocio (B2C o B2B).
     - `year` (string): Año.
     - `month` (string): Mes.
     - `dayOfMonth` (string): Día del mes.
     - `hour` (string): Hora.
     - `guid` (string): GUID del documento.
   - **Cuerpo de la Solicitud**: XML del documento.
   - **Respuestas**:
     - **200 OK**: Documento guardado con éxito.
     - **400 Bad Request**: XML no válido.
     - **500 Internal Server Error**: Error al guardar el documento.

### **Estructura del Proyecto**

1. **`RedisConfiguration.cs`**
   - Contiene la configuración de conexión a Redis.

2. **`DocumentPropertiesGET.cs`**
   - Modelo para obtener las propiedades de un documento almacenado en Redis.

3. **`DocumentPropertiesPUT.cs`**
   - Modelo para guardar las propiedades de un documento en Redis.

4. **`Document.cs`**
   - Contiene la lógica para procesar el XML del documento y guardar sus propiedades en Redis.

5. **`StorageHelper.cs`**
   - Contiene métodos auxiliares para guardar documentos en el sistema de archivos local.

6. **`XmlDocsController.cs`**
   - Controlador principal que maneja las solicitudes HTTP para gestionar documentos.

### **Pasos para Ejecutar el Proyecto Usando Swagger**

1. **Clonar el Repositorio**
   - Clona el repositorio en tu máquina local.
     ```sh
     git clone https://github.com/usuario/proyecto.git
     cd proyecto
     ```

2. **Configurar Redis**
   - Asegúrate de tener una instancia de Redis en funcionamiento.
   - Configura los detalles de conexión en el archivo `appsettings.json`:
     ```json
     {
       "RedisSettings": {
         "KeyPrefix": "dgii:document",
         "RedisHost": "redis-host",
         "RedisPassword": "redis-password",
         "RedisPort": 6379,
         "RedisUsername": "default"
       },
       "StorageBasePath": "C:\\Path\\To\\Storage"
     }
     ```

3. **Compilar el Proyecto**
   - Navega a la raíz del proyecto y compílalo para asegurarte de que no hay errores.
     ```sh
     dotnet build
     ```

4. **Ejecutar el Proyecto**
   - Ejecuta el proyecto en modo de desarrollo.
     ```sh
     dotnet run
     ```

5. **Abrir Swagger**
   - Una vez que el proyecto esté en ejecución, abre un navegador web y navega a `https://localhost:7158/swagger` o `http://localhost:5164/swagger` (la URL exacta puede variar según tu configuración).
   - Verás la interfaz de Swagger que lista todos los endpoints disponibles en la API.

### **Probar la API Usando Swagger**

1. **Guardar un Documento (PUT /api/xml-docs/document/{businessModel}/{year}/{month}/{dayOfMonth}/{hour}/{guid})**

   - En la interfaz de Swagger, encuentra el endpoint `PUT /api/xml-docs/document/{businessModel}/{year}/{month}/{dayOfMonth}/{hour}/{guid}`.
   - Haz clic en el botón `Try it out`.
   - Llena los parámetros requeridos:
     - `businessModel`: Elige entre `B2C` o `B2B`.
     - `year`: Año (por ejemplo, `2024`).
     - `month`: Mes (por ejemplo, `05`).
     - `dayOfMonth`: Día del mes (por ejemplo, `27`).
     - `hour`: Hora (por ejemplo, `14`).
     - `guid`: Un GUID único (por ejemplo, `unique-guid`).
   - En el cuerpo de la solicitud (`Request Body`), proporciona el XML del documento. Por ejemplo:
     ```xml
     <Document>
       <ClientId>123456789</ClientId>
       <DocumentName>example.xml</DocumentName>
     </Document>
     ```
   - Haz clic en el botón `Execute`.

2. **Recuperar un Documento (GET /api/xml-docs/document/{documentId})**

   - En la interfaz de Swagger, encuentra el endpoint `GET /api/xml-docs/document/{documentId}`.
   - Haz clic en el botón `Try it out`.
   - Llena el parámetro `documentId` con el ID del documento que deseas recuperar (por ejemplo, `unique-guid`).
   - Haz clic en el botón `Execute`.

### **Ejecutar el Proyecto Usando Visual Studio**

1. **Abrir el Proyecto en Visual Studio**
   - Abre Visual Studio.
   - Selecciona `File` > `Open` > `Project/Solution`.
   - Navega al directorio del proyecto clonado y selecciona el archivo de solución `.sln`.

2. **Configurar Redis**
   - Asegúrate de tener una instancia de Redis en funcionamiento.
   - Configura los detalles de conexión en el archivo `appsettings.json`:
     ```json
     {
       "RedisSettings": {
         "KeyPrefix": "dgii:document",
         "RedisHost": "redis-host",
         "RedisPassword": "redis-password",
         "RedisPort": 6379,
         "RedisUsername": "default"
       },
       "StorageBasePath": "C:\\Path\\To\\Storage"
     }
     ```

3. **Compilar el Proyecto**
   - Asegúrate de que la configuración de compilación esté configurada en `Debug`.
   - Haz clic en `Build` > `Build Solution`.

4. **Ejecutar el Proyecto**
   - Haz clic en el botón `IIS Express` o `Play` en la barra de herramientas de Visual Studio para iniciar el proyecto.
   - Visual Studio abrirá automáticamente un navegador web y navegará a `https://localhost:7158/swagger` o `http://localhost:5164/swagger` (la URL exacta puede variar según tu configuración).

### **Probar la API Usando Swagger en Visual Studio**

Sigue los mismos pasos de prueba descritos anteriormente utilizando la interfaz de Swagger que se abre automáticamente en el navegador web.

### **Notas Adicionales**

- Asegúrate de que el servicio de Redis esté accesible desde tu entorno de desarrollo.
- Revisa los logs del proyecto para depurar posibles problemas de conexión o almacenamiento.
- Configura las rutas de almacenamiento adecuadamente en el archivo `appsettings.json` para que los documentos se guarden en el lugar correcto.

Con esta guía, deberías ser capaz de ejecutar y probar la API `XmlDocs` usando tanto la interfaz de Swagger como Visual Studio en tu entorno local.
