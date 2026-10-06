# Servicios — Sistema profesional de recibos mensuales

## Iniciar en Windows (Visual Studio o VS Code)
1. Extrae TODO el ZIP en C:\Proyectos\Servicios. No ejecutes dentro del ZIP.
2. Instala el SDK de .NET 8 si aún no lo tienes.
3. Abre una terminal dentro de BoletasProfesional (donde está BoletasAguaLuzRazor.csproj).
4. Ejecuta: dotnet restore
5. Ejecuta: dotnet run
6. Abre http://localhost:5180.
7. En el primer inicio selecciona “Configurar primer administrador”. Elige tu propio usuario y una contraseña de al menos 10 caracteres. No existen contraseñas predeterminadas.

## Preparación del administrador
- Usuarios: registra nombre, usuario, teléfono, habitación y contraseña inicial. Entrega estas credenciales al residente para que inicie sesión. Puede cambiarlas en Mi contraseña.
- Configuración Yape: introduce el nombre de tu establecimiento, el celular real del receptor y su titular. Sube el QR original de tu aplicación Yape (PNG o JPG).
- Recibos mensuales: selecciona residente, año, mes y vencimiento; selecciona UN servicio y su importe. No se permite repetir usuario/año/mes/servicio.
- Cada mes se emite un recibo nuevo. No se combinan varios meses en un documento. Agua, luz, internet y cuarto se emiten, descargan y pagan en recibos distintos.
- Pagos: revisa el comprobante, verifica el abono en TU cuenta Yape y aprueba o rechaza. Para rechazar, indica el motivo. Al aprobar el recibo queda pagado. Al rechazar vuelve a pendiente y el residente puede enviar otra captura.

## Uso del residente
1. Ingresa con el usuario y contraseña entregados.
2. Abre Mis recibos y filtra por año, mes o estado.
3. Consulta el detalle o descarga el PDF individual del período.
4. Abre Yape, escanea el QR o ingresa el celular, verifica el titular y paga el total.
5. Registra la referencia de operación y adjunta la captura PNG/JPG (máximo 5 MB).
6. Espera la revisión. Una captura enviada no se considera un pago confirmado.

## Funciones y reglas
- Roles administrador / residente; consultas y archivos restringidos al propietario o al administrador.
- Contraseñas con hash de ASP.NET Core, cookies protegidas y formularios con antifalsificación.
- Bloqueo de 15 minutos tras cinco contraseñas incorrectas para una cuenta existente.
- Cambiar o restablecer una contraseña invalida las sesiones anteriores.
- Usuario sin recibos: eliminación definitiva. Usuario con historial: se elimina su acceso conservando los recibos y pagos; puede reactivarse.
- Comprobantes privados guardados en la base de datos, fuera de wwwroot.
- Capturas rechazadas se conservan para seguimiento; no se reutiliza una referencia que está aprobada o en revisión.
- El PDF es una constancia interna, no un comprobante tributario de SUNAT.

## Datos y respaldo
SQLite crea App_Data/servicios.db automáticamente; no requiere SQL Server ni migraciones manuales en el primer inicio. No borres ese archivo: contiene usuarios, recibos, pagos, QR y configuración. Haz copias con el sistema detenido. Para compartir acceso entre varios dispositivos despliega UNA instancia central; no copies la base activa entre computadoras.

## Proyecto anterior
El ZIP incluye RespaldoOriginal con el código y archivos de datos encontrados del proyecto entregado, excluyendo bin, obj y .vs. La versión profesional usa una base nueva porque el esquema anterior no tiene cuentas ni períodos mensuales. Los recibos anteriores no se importan automáticamente: asígnalos a sus usuarios y meses al registrarlos en el nuevo sistema. Conserva tu base anterior.

## Publicar
Primero configura el administrador localmente. Para acceso público utiliza un servidor compatible con ASP.NET Core y HTTPS, almacenamiento persistente y copias de seguridad; configura ASPNETCORE_ENVIRONMENT=Production. No publiques una instalación sin configurar su primer administrador. El puerto 5180 es para desarrollo local.

## Verificación de esta entrega
Se verificó con SQLite la actualización de esquema, conservación de recibos anteriores y rechazo de duplicados por servicio. Se revisaron estáticamente las rutas, autorización, validaciones y estructura del ZIP. No fue posible compilar ni ejecutar aquí: este entorno no tiene el SDK de .NET y el acceso para descargarlo no estuvo disponible. Ejecuta dotnet restore y dotnet build en tu equipo antes de usarlo con datos reales.

## Actualizar desde la primera versión profesional
1. Detén el sistema y haz una copia de tu carpeta App_Data.
2. Extrae este ZIP en una carpeta nueva.
3. Copia tu carpeta App_Data anterior dentro de BoletasProfesional.
4. Inicia el proyecto. Actualiza la base automáticamente sin borrar cuentas ni pagos.
5. Los recibos anteriores se muestran como General / Anteriores; conservan importes, comprobantes y estados. Los nuevos recibos se emiten por servicio.

Las ilustraciones SVG de los servicios están incluidas localmente; funcionan sin depender de sitios de imágenes externos. Esta actualización incluye las correcciones Razor de Emitir e Index y el mensaje de validación del usuario en español.

## Versión 3: archivo mensual y gráfico anual
- Recibos: 12 ventanas de enero a diciembre para cada año. Cada ventana muestra número de recibos e importe total de los servicios seleccionados. El estado filtra el listado inferior.
- Resumen: selector de año y gráfico de barras de los importes emitidos, agrupados por mes de servicio. No corresponde a la fecha del pago: incluye recibos pendientes, en revisión y pagados. El administrador ve todos los residentes; cada residente solo sus propios importes. Las barras abren el mes y muestran sus valores exactos en una tabla accesible.
- Eliminar: disponible solo para el administrador en las tarjetas de recibos. Requiere confirmar. Se oculta el recibo y se excluye de los gráficos; se conserva internamente el historial. Puede emitirse un recibo nuevo del mismo usuario, servicio y mes.
- Los archivos de Font Awesome Free están incluidos localmente; no necesitan conexión. Se usa pen-to-square Regular para editar y Solid/Regular para las demás acciones. El estilo Light necesita Font Awesome Pro y no se incluye sin licencia.
- Este ZIP conserva la base de datos adjunta, consolidada en un archivo sin depender de WAL. Si ya registraste nuevos datos después de enviarlo, usa tu carpeta App_Data más reciente, con el sistema detenido y una copia de respaldo.
- Verificación: se probó en una copia SQLite que la actualización mantiene los 11 recibos adjuntos, permite reemitir un período eliminado y rechaza duplicados activos. La compilación ASP.NET Core no se pudo ejecutar en este entorno por falta del SDK de .NET.

## Pantallas independientes por mes
Recibos muestra solamente las doce tarjetas del año. Al abrir una tarjeta se navega a otra pantalla: /Recibos/Mes/2026/1 para enero, /Recibos/Mes/2026/2 para febrero, etc. Cada pantalla consulta exclusivamente los recibos de su año y mes y permite filtrar, abrir PDF, pagar o eliminar según el rol. Volver a los meses regresa al selector. Las barras del gráfico abren directamente su pantalla mensual. La base de datos no cambia en esta actualización.

## Gráficos separados por servicio
Resumen muestra cuatro gráficos: Agua, Luz, Internet y Cuartos. Cada gráfico tiene 12 barras, total anual y mes o meses de mayor importe. Cada uno usa su propia escala. Se suman las columnas de importes del servicio correspondiente, incluyendo el desglose de los recibos antiguos General. Las barras enlazan a los recibos de su mes filtrados por servicio. Los importes son cargos emitidos, no únicamente pagos confirmados. No se cambia la base de datos.
