IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ZapateriaBD].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ZapateriaBD] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ZapateriaBD] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ZapateriaBD] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ZapateriaBD] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ZapateriaBD] SET ARITHABORT OFF 
GO
ALTER DATABASE [ZapateriaBD] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ZapateriaBD] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ZapateriaBD] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ZapateriaBD] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ZapateriaBD] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ZapateriaBD] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ZapateriaBD] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ZapateriaBD] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ZapateriaBD] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ZapateriaBD] SET  ENABLE_BROKER 
GO
ALTER DATABASE [ZapateriaBD] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ZapateriaBD] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ZapateriaBD] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ZapateriaBD] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ZapateriaBD] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ZapateriaBD] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ZapateriaBD] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ZapateriaBD] SET RECOVERY FULL 
GO
ALTER DATABASE [ZapateriaBD] SET  MULTI_USER 
GO
ALTER DATABASE [ZapateriaBD] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ZapateriaBD] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ZapateriaBD] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ZapateriaBD] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [ZapateriaBD] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [ZapateriaBD] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'ZapateriaBD', N'ON'
GO
ALTER DATABASE [ZapateriaBD] SET QUERY_STORE = ON
GO
ALTER DATABASE [ZapateriaBD] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [ZapateriaBD]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_CalcularDescuentoPorCantidad]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------------------------------------------------------------------------------------------

-- 6. fn_CalcularDescuentoPorCantidad

-- Devuelve el descuento basado en cantidad (5% si compra >10, 10% si >20).

CREATE FUNCTION [dbo].[fn_CalcularDescuentoPorCantidad] (@cantidad INT, @precioUnitario DECIMAL(10,2))
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @descuento DECIMAL(10,2);
    IF @cantidad > 20
        SET @descuento = @precioUnitario * 0.10;
    ELSE IF @cantidad > 10
        SET @descuento = @precioUnitario * 0.05;
    ELSE
        SET @descuento = 0;
    RETURN @descuento;
END;
-- Regla: Aplicar descuento automático por cantidad comprada.
GO
/****** Object:  UserDefinedFunction [dbo].[fn_CalcularPrecioConImpuesto]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- FUNCIONES

-- 1. fn_CalcularPrecioConImpuesto

-- Devuelve el precio final sumando un 13% de IVA al precio base.

CREATE FUNCTION [dbo].[fn_CalcularPrecioConImpuesto] (@precioBase DECIMAL(10,2))
RETURNS DECIMAL(10,2)
AS
BEGIN
    RETURN @precioBase * 1.13;
END;
-- Regla: Calcular automáticamente precio final con impuesto.
GO
/****** Object:  UserDefinedFunction [dbo].[fn_ObtenerCantidadStockProducto]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--------------------------------------------------------------------------------------------------------------

-- 2.  fn_ObtenerCantidadStockProducto

-- Devuelve el stock actual de un producto.

CREATE FUNCTION [dbo].[fn_ObtenerCantidadStockProducto] (@idProducto INT)
RETURNS INT
AS
BEGIN
    DECLARE @stock INT;
    SELECT @stock = Stock FROM Productos WHERE ID_Producto = @idProducto;
    RETURN @stock;
END;
-- Regla: Consultar stock rápidamente.
GO
/****** Object:  UserDefinedFunction [dbo].[fn_ProductosPorCategoria]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--------------------------------------------------------------------------------------------------------------

-- 5. fn_ProductosPorCategoria

-- Devuelve el número de productos por categoría.

CREATE FUNCTION [dbo].[fn_ProductosPorCategoria] (@idCategoria INT)
RETURNS INT
AS
BEGIN
    RETURN (SELECT COUNT(*) FROM Productos WHERE ID_Categoria = @idCategoria);
END;
-- Regla: Consultar inventario por categoría.
GO
/****** Object:  UserDefinedFunction [dbo].[fn_StockCritico]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--------------------------------------------------------------------------------------------------------------

-- 8. fn_StockCritico

-- Devuelve 1 si stock <= mínimo, 0 si no.

CREATE FUNCTION [dbo].[fn_StockCritico] (@idProducto INT)
RETURNS BIT
AS
BEGIN
    DECLARE @stock INT;
    SELECT @stock = Stock FROM Productos WHERE ID_Producto = @idProducto;
    IF @stock <= 5
        RETURN 1;
    RETURN 0;
END;
-- Regla: Detectar productos con stock crítico.
GO
/****** Object:  UserDefinedFunction [dbo].[fn_StockDisponiblePorCategoria]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--------------------------------------------------------------------------------------------------------------

-- 3. fn_StockDisponiblePorCategoria

-- Devuelve el stock total disponible para una categoría específica.

CREATE FUNCTION [dbo].[fn_StockDisponiblePorCategoria] (@idCategoria INT)
RETURNS INT
AS
BEGIN
    DECLARE @stockTotal INT;
    SELECT @stockTotal = SUM(Stock) FROM Productos WHERE ID_Categoria = @idCategoria;
    RETURN ISNULL(@stockTotal, 0);
END;
-- Regla: Consultar stock total por categoría.
GO
/****** Object:  UserDefinedFunction [dbo].[fn_TotalVendidoPorProducto]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--------------------------------------------------------------------------------------------------------------

-- 4. fn_TotalVendidoPorProducto

-- Devuelve la cantidad total vendida de un producto específico.

CREATE FUNCTION [dbo].[fn_TotalVendidoPorProducto] (@idProducto INT)
RETURNS INT
AS
BEGIN
    DECLARE @totalVendido INT;
    SELECT @totalVendido = SUM(Cantidad) FROM Detalle_Venta WHERE ID_Producto = @idProducto;
    RETURN ISNULL(@totalVendido, 0);
END;
-- Regla: Consultar total vendido por producto.
GO
/****** Object:  UserDefinedFunction [dbo].[fn_VentasPorEmpleado]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------------------------------------------------------------------------------------------------

-- 7. fn_VentasPorEmpleado

-- Devuelve el número de ventas realizadas por un empleado específico.

CREATE FUNCTION [dbo].[fn_VentasPorEmpleado] (@idEmpleado INT)
RETURNS INT
AS
BEGIN
    DECLARE @ventas INT;
    SELECT @ventas = COUNT(*) FROM Ventas WHERE ID_Empleado = @idEmpleado;
    RETURN ISNULL(@ventas, 0);
END;
-- Regla: Consultar ventas por empleado.
GO
/****** Object:  Table [dbo].[Categoria]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categoria](
	[ID_Categoria] [int] IDENTITY(1,1) NOT NULL,
	[Nombre_Categoria] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID_Categoria] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Detalle_Venta]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Detalle_Venta](
	[ID_Detalle_Venta] [int] IDENTITY(1,1) NOT NULL,
	[ID_Venta] [int] NOT NULL,
	[ID_Producto] [int] NOT NULL,
	[Cantidad] [int] NOT NULL,
	[Precio_Unitario] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID_Detalle_Venta] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Empleados]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Empleados](
	[ID_Empleado] [int] IDENTITY(1,1) NOT NULL,
	[Nombre_Empleado] [varchar](50) NOT NULL,
	[Apellido_Empleado] [varchar](50) NOT NULL,
	[Direccion] [varchar](200) NULL,
	[Telefono] [varchar](20) NULL,
	[Correo] [varchar](100) NULL,
	[Posicion] [varchar](50) NULL,
	[Salario] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID_Empleado] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Productos]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Productos](
	[ID_Producto] [int] IDENTITY(1,1) NOT NULL,
	[Nombre_Producto] [varchar](100) NOT NULL,
	[Talla] [varchar](10) NOT NULL,
	[Modelo] [varchar](50) NULL,
	[Marca] [varchar](50) NULL,
	[Color] [varchar](30) NULL,
	[Precio_Unitario] [decimal](10, 2) NOT NULL,
	[Material] [varchar](50) NULL,
	[Stock] [int] NOT NULL,
	[ID_Categoria] [int] NOT NULL,
	[ID_Proveedor] [int] NOT NULL,
	[Imagen_Producto] [varchar](200) NULL,
	[Codigo_Barra] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID_Producto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Proveedores]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Proveedores](
	[ID_Proveedor] [int] IDENTITY(1,1) NOT NULL,
	[Nombre_Proveedor] [varchar](100) NOT NULL,
	[Direccion] [varchar](200) NULL,
	[Telefono] [varchar](20) NULL,
	[Correo_Proveedor] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID_Proveedor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ventas]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ventas](
	[ID_Venta] [int] IDENTITY(1,1) NOT NULL,
	[Hora_Fecha] [datetime] NOT NULL,
	[Total_Precio] [decimal](10, 2) NOT NULL,
	[ID_Empleado] [int] NOT NULL,
	[Metodo_Pago] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID_Venta] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Categoria] ON 

INSERT [dbo].[Categoria] ([ID_Categoria], [Nombre_Categoria]) VALUES (1, N'Hombre')
INSERT [dbo].[Categoria] ([ID_Categoria], [Nombre_Categoria]) VALUES (2, N'Mujer')
INSERT [dbo].[Categoria] ([ID_Categoria], [Nombre_Categoria]) VALUES (3, N'Niño')
INSERT [dbo].[Categoria] ([ID_Categoria], [Nombre_Categoria]) VALUES (4, N'Niña')
INSERT [dbo].[Categoria] ([ID_Categoria], [Nombre_Categoria]) VALUES (5, N'Deportivo')
INSERT [dbo].[Categoria] ([ID_Categoria], [Nombre_Categoria]) VALUES (6, N'Casual')
INSERT [dbo].[Categoria] ([ID_Categoria], [Nombre_Categoria]) VALUES (7, N'Formal')
INSERT [dbo].[Categoria] ([ID_Categoria], [Nombre_Categoria]) VALUES (8, N'Sandalias')
INSERT [dbo].[Categoria] ([ID_Categoria], [Nombre_Categoria]) VALUES (9, N'Botas')
INSERT [dbo].[Categoria] ([ID_Categoria], [Nombre_Categoria]) VALUES (10, N'Pantuflas')
SET IDENTITY_INSERT [dbo].[Categoria] OFF
GO
SET IDENTITY_INSERT [dbo].[Detalle_Venta] ON 

INSERT [dbo].[Detalle_Venta] ([ID_Detalle_Venta], [ID_Venta], [ID_Producto], [Cantidad], [Precio_Unitario]) VALUES (1, 1, 1, 1, CAST(1299.99 AS Decimal(10, 2)))
INSERT [dbo].[Detalle_Venta] ([ID_Detalle_Venta], [ID_Venta], [ID_Producto], [Cantidad], [Precio_Unitario]) VALUES (2, 1, 5, 1, CAST(1599.50 AS Decimal(10, 2)))
INSERT [dbo].[Detalle_Venta] ([ID_Detalle_Venta], [ID_Venta], [ID_Producto], [Cantidad], [Precio_Unitario]) VALUES (3, 2, 2, 2, CAST(899.50 AS Decimal(10, 2)))
INSERT [dbo].[Detalle_Venta] ([ID_Detalle_Venta], [ID_Venta], [ID_Producto], [Cantidad], [Precio_Unitario]) VALUES (4, 2, 3, 1, CAST(499.75 AS Decimal(10, 2)))
INSERT [dbo].[Detalle_Venta] ([ID_Detalle_Venta], [ID_Venta], [ID_Producto], [Cantidad], [Precio_Unitario]) VALUES (5, 3, 4, 1, CAST(1899.00 AS Decimal(10, 2)))
INSERT [dbo].[Detalle_Venta] ([ID_Detalle_Venta], [ID_Venta], [ID_Producto], [Cantidad], [Precio_Unitario]) VALUES (6, 4, 6, 1, CAST(1799.00 AS Decimal(10, 2)))
INSERT [dbo].[Detalle_Venta] ([ID_Detalle_Venta], [ID_Venta], [ID_Producto], [Cantidad], [Precio_Unitario]) VALUES (7, 4, 7, 1, CAST(699.50 AS Decimal(10, 2)))
INSERT [dbo].[Detalle_Venta] ([ID_Detalle_Venta], [ID_Venta], [ID_Producto], [Cantidad], [Precio_Unitario]) VALUES (8, 5, 8, 2, CAST(799.00 AS Decimal(10, 2)))
INSERT [dbo].[Detalle_Venta] ([ID_Detalle_Venta], [ID_Venta], [ID_Producto], [Cantidad], [Precio_Unitario]) VALUES (9, 5, 9, 1, CAST(2199.00 AS Decimal(10, 2)))
INSERT [dbo].[Detalle_Venta] ([ID_Detalle_Venta], [ID_Venta], [ID_Producto], [Cantidad], [Precio_Unitario]) VALUES (10, 6, 10, 3, CAST(899.00 AS Decimal(10, 2)))
INSERT [dbo].[Detalle_Venta] ([ID_Detalle_Venta], [ID_Venta], [ID_Producto], [Cantidad], [Precio_Unitario]) VALUES (11, 6, 11, 2, CAST(399.00 AS Decimal(10, 2)))
INSERT [dbo].[Detalle_Venta] ([ID_Detalle_Venta], [ID_Venta], [ID_Producto], [Cantidad], [Precio_Unitario]) VALUES (12, 7, 12, 1, CAST(1399.00 AS Decimal(10, 2)))
INSERT [dbo].[Detalle_Venta] ([ID_Detalle_Venta], [ID_Venta], [ID_Producto], [Cantidad], [Precio_Unitario]) VALUES (13, 7, 1, 1, CAST(1299.99 AS Decimal(10, 2)))
INSERT [dbo].[Detalle_Venta] ([ID_Detalle_Venta], [ID_Venta], [ID_Producto], [Cantidad], [Precio_Unitario]) VALUES (14, 8, 2, 1, CAST(899.50 AS Decimal(10, 2)))
INSERT [dbo].[Detalle_Venta] ([ID_Detalle_Venta], [ID_Venta], [ID_Producto], [Cantidad], [Precio_Unitario]) VALUES (15, 8, 5, 1, CAST(1599.50 AS Decimal(10, 2)))
INSERT [dbo].[Detalle_Venta] ([ID_Detalle_Venta], [ID_Venta], [ID_Producto], [Cantidad], [Precio_Unitario]) VALUES (16, 9, 7, 2, CAST(699.50 AS Decimal(10, 2)))
INSERT [dbo].[Detalle_Venta] ([ID_Detalle_Venta], [ID_Venta], [ID_Producto], [Cantidad], [Precio_Unitario]) VALUES (17, 9, 10, 1, CAST(899.00 AS Decimal(10, 2)))
INSERT [dbo].[Detalle_Venta] ([ID_Detalle_Venta], [ID_Venta], [ID_Producto], [Cantidad], [Precio_Unitario]) VALUES (18, 10, 3, 2, CAST(499.75 AS Decimal(10, 2)))
SET IDENTITY_INSERT [dbo].[Detalle_Venta] OFF
GO
SET IDENTITY_INSERT [dbo].[Empleados] ON 

INSERT [dbo].[Empleados] ([ID_Empleado], [Nombre_Empleado], [Apellido_Empleado], [Direccion], [Telefono], [Correo], [Posicion], [Salario]) VALUES (1, N'Juan', N'Pérez', N'Calle 1 #10', N'555-1111', N'juan.perez@email.com', N'Vendedor', CAST(12000.00 AS Decimal(10, 2)))
INSERT [dbo].[Empleados] ([ID_Empleado], [Nombre_Empleado], [Apellido_Empleado], [Direccion], [Telefono], [Correo], [Posicion], [Salario]) VALUES (2, N'María', N'González', N'Calle 2 #20', N'555-2222', N'maria.gonzalez@email.com', N'Cajera', CAST(10000.00 AS Decimal(10, 2)))
INSERT [dbo].[Empleados] ([ID_Empleado], [Nombre_Empleado], [Apellido_Empleado], [Direccion], [Telefono], [Correo], [Posicion], [Salario]) VALUES (3, N'Carlos', N'Rodríguez', N'Calle 3 #30', N'555-3333', N'carlos.rodriguez@email.com', N'Gerente', CAST(20000.00 AS Decimal(10, 2)))
INSERT [dbo].[Empleados] ([ID_Empleado], [Nombre_Empleado], [Apellido_Empleado], [Direccion], [Telefono], [Correo], [Posicion], [Salario]) VALUES (4, N'Ana', N'López', N'Calle 4 #40', N'555-4444', N'ana.lopez@email.com', N'Vendedora', CAST(12000.00 AS Decimal(10, 2)))
INSERT [dbo].[Empleados] ([ID_Empleado], [Nombre_Empleado], [Apellido_Empleado], [Direccion], [Telefono], [Correo], [Posicion], [Salario]) VALUES (5, N'Roberto', N'Martínez', N'Calle 5 #50', N'555-5555', N'roberto.martinez@email.com', N'Almacenista', CAST(9000.00 AS Decimal(10, 2)))
INSERT [dbo].[Empleados] ([ID_Empleado], [Nombre_Empleado], [Apellido_Empleado], [Direccion], [Telefono], [Correo], [Posicion], [Salario]) VALUES (6, N'Laura', N'Sánchez', N'Calle 6 #60', N'555-6666', N'laura.sanchez@email.com', N'Asistente', CAST(8500.00 AS Decimal(10, 2)))
INSERT [dbo].[Empleados] ([ID_Empleado], [Nombre_Empleado], [Apellido_Empleado], [Direccion], [Telefono], [Correo], [Posicion], [Salario]) VALUES (7, N'Miguel', N'Fernández', N'Calle 7 #70', N'555-7777', N'miguel.fernandez@email.com', N'Vendedor', CAST(12000.00 AS Decimal(10, 2)))
INSERT [dbo].[Empleados] ([ID_Empleado], [Nombre_Empleado], [Apellido_Empleado], [Direccion], [Telefono], [Correo], [Posicion], [Salario]) VALUES (8, N'Sofía', N'Díaz', N'Calle 8 #80', N'555-8888', N'sofia.diaz@email.com', N'Cajera', CAST(10000.00 AS Decimal(10, 2)))
INSERT [dbo].[Empleados] ([ID_Empleado], [Nombre_Empleado], [Apellido_Empleado], [Direccion], [Telefono], [Correo], [Posicion], [Salario]) VALUES (9, N'Pedro', N'Ramírez', N'Calle 9 #90', N'555-9999', N'pedro.ramirez@email.com', N'Vendedor', CAST(12000.00 AS Decimal(10, 2)))
INSERT [dbo].[Empleados] ([ID_Empleado], [Nombre_Empleado], [Apellido_Empleado], [Direccion], [Telefono], [Correo], [Posicion], [Salario]) VALUES (10, N'Carmen', N'Torres', N'Calle 10 #100', N'555-0000', N'carmen.torres@email.com', N'Vendedora', CAST(12000.00 AS Decimal(10, 2)))
SET IDENTITY_INSERT [dbo].[Empleados] OFF
GO
SET IDENTITY_INSERT [dbo].[Productos] ON 

INSERT [dbo].[Productos] ([ID_Producto], [Nombre_Producto], [Talla], [Modelo], [Marca], [Color], [Precio_Unitario], [Material], [Stock], [ID_Categoria], [ID_Proveedor], [Imagen_Producto], [Codigo_Barra]) VALUES (1, N'Zapato Casual', N'42', N'Milano', N'Comfort', N'Negro', CAST(1299.99 AS Decimal(10, 2)), N'Cuero', 25, 1, 1, NULL, N'ZC-001-42-N')
INSERT [dbo].[Productos] ([ID_Producto], [Nombre_Producto], [Talla], [Modelo], [Marca], [Color], [Precio_Unitario], [Material], [Stock], [ID_Categoria], [ID_Proveedor], [Imagen_Producto], [Codigo_Barra]) VALUES (2, N'Tenis Deportivo', N'38', N'Runner', N'Sprint', N'Azul', CAST(899.50 AS Decimal(10, 2)), N'Sintético', 30, 2, 2, NULL, N'TD-002-38-A')
INSERT [dbo].[Productos] ([ID_Producto], [Nombre_Producto], [Talla], [Modelo], [Marca], [Color], [Precio_Unitario], [Material], [Stock], [ID_Categoria], [ID_Proveedor], [Imagen_Producto], [Codigo_Barra]) VALUES (3, N'Sandalia', N'24', N'Summer', N'KidStep', N'Rosa', CAST(499.75 AS Decimal(10, 2)), N'Sintético', 15, 4, 3, NULL, N'SN-003-24-R')
INSERT [dbo].[Productos] ([ID_Producto], [Nombre_Producto], [Talla], [Modelo], [Marca], [Color], [Precio_Unitario], [Material], [Stock], [ID_Categoria], [ID_Proveedor], [Imagen_Producto], [Codigo_Barra]) VALUES (4, N'Bota de Trabajo', N'44', N'Worker', N'Safety', N'Café', CAST(1899.00 AS Decimal(10, 2)), N'Cuero', 10, 1, 1, NULL, N'BT-004-44-C')
INSERT [dbo].[Productos] ([ID_Producto], [Nombre_Producto], [Talla], [Modelo], [Marca], [Color], [Precio_Unitario], [Material], [Stock], [ID_Categoria], [ID_Proveedor], [Imagen_Producto], [Codigo_Barra]) VALUES (5, N'Zapatilla Elegante', N'39', N'Fashion', N'Glamour', N'Rojo', CAST(1599.50 AS Decimal(10, 2)), N'Charol', 12, 2, 2, NULL, N'ZE-005-39-R')
INSERT [dbo].[Productos] ([ID_Producto], [Nombre_Producto], [Talla], [Modelo], [Marca], [Color], [Precio_Unitario], [Material], [Stock], [ID_Categoria], [ID_Proveedor], [Imagen_Producto], [Codigo_Barra]) VALUES (6, N'Zapato Formal', N'41', N'Executive', N'Business', N'Negro', CAST(1799.00 AS Decimal(10, 2)), N'Cuero', 18, 7, 4, NULL, N'ZF-006-41-N')
INSERT [dbo].[Productos] ([ID_Producto], [Nombre_Producto], [Talla], [Modelo], [Marca], [Color], [Precio_Unitario], [Material], [Stock], [ID_Categoria], [ID_Proveedor], [Imagen_Producto], [Codigo_Barra]) VALUES (7, N'Tenis para Niño', N'30', N'Explorer', N'KidFun', N'Multicolor', CAST(699.50 AS Decimal(10, 2)), N'Sintético', 22, 3, 5, NULL, N'TN-007-30-M')
INSERT [dbo].[Productos] ([ID_Producto], [Nombre_Producto], [Talla], [Modelo], [Marca], [Color], [Precio_Unitario], [Material], [Stock], [ID_Categoria], [ID_Proveedor], [Imagen_Producto], [Codigo_Barra]) VALUES (8, N'Sandalia para Dama', N'37', N'Beach', N'SummerStep', N'Dorado', CAST(799.00 AS Decimal(10, 2)), N'Sintético', 14, 8, 6, NULL, N'SD-008-37-D')
INSERT [dbo].[Productos] ([ID_Producto], [Nombre_Producto], [Talla], [Modelo], [Marca], [Color], [Precio_Unitario], [Material], [Stock], [ID_Categoria], [ID_Proveedor], [Imagen_Producto], [Codigo_Barra]) VALUES (9, N'Bota para Invierno', N'40', N'Winter', N'WarmFoot', N'Negro', CAST(2199.00 AS Decimal(10, 2)), N'Cuero y Lana', 8, 9, 7, NULL, N'BI-009-40-N')
INSERT [dbo].[Productos] ([ID_Producto], [Nombre_Producto], [Talla], [Modelo], [Marca], [Color], [Precio_Unitario], [Material], [Stock], [ID_Categoria], [ID_Proveedor], [Imagen_Producto], [Codigo_Barra]) VALUES (10, N'Zapato Escolar', N'33', N'Student', N'SchoolStep', N'Negro', CAST(899.00 AS Decimal(10, 2)), N'Cuero Sintético', 35, 3, 8, NULL, N'ZE-010-33-N')
INSERT [dbo].[Productos] ([ID_Producto], [Nombre_Producto], [Talla], [Modelo], [Marca], [Color], [Precio_Unitario], [Material], [Stock], [ID_Categoria], [ID_Proveedor], [Imagen_Producto], [Codigo_Barra]) VALUES (11, N'Pantufla', N'43', N'Comfort', N'HomeStep', N'Azul', CAST(399.00 AS Decimal(10, 2)), N'Tela', 20, 10, 9, NULL, N'PT-011-43-A')
INSERT [dbo].[Productos] ([ID_Producto], [Nombre_Producto], [Talla], [Modelo], [Marca], [Color], [Precio_Unitario], [Material], [Stock], [ID_Categoria], [ID_Proveedor], [Imagen_Producto], [Codigo_Barra]) VALUES (12, N'Zapato de Salón', N'38', N'Dance', N'PartyStep', N'Plateado', CAST(1399.00 AS Decimal(10, 2)), N'Sintético', 10, 2, 2, NULL, N'ZS-012-38-P')
INSERT [dbo].[Productos] ([ID_Producto], [Nombre_Producto], [Talla], [Modelo], [Marca], [Color], [Precio_Unitario], [Material], [Stock], [ID_Categoria], [ID_Proveedor], [Imagen_Producto], [Codigo_Barra]) VALUES (13, N'Zapato Test', N'42', N'ModeloX', N'MarcaY', N'Negro', CAST(100.00 AS Decimal(10, 2)), N'Cuero', 10, 1, 1, NULL, N'EXISTENTE')
SET IDENTITY_INSERT [dbo].[Productos] OFF
GO
SET IDENTITY_INSERT [dbo].[Proveedores] ON 

INSERT [dbo].[Proveedores] ([ID_Proveedor], [Nombre_Proveedor], [Direccion], [Telefono], [Correo_Proveedor]) VALUES (1, N'Calzados Premium S.A.', N'Av. Principal #123, Ciudad', N'555-1234', N'ventas@calzadospremium.com')
INSERT [dbo].[Proveedores] ([ID_Proveedor], [Nombre_Proveedor], [Direccion], [Telefono], [Correo_Proveedor]) VALUES (2, N'Importadora Zapatos', N'Calle Comercio #456, Ciudad', N'555-5678', N'pedidos@importadorazapatos.com')
INSERT [dbo].[Proveedores] ([ID_Proveedor], [Nombre_Proveedor], [Direccion], [Telefono], [Correo_Proveedor]) VALUES (3, N'Distribuidora Calzados', N'Av. Industrial #789, Ciudad', N'555-9012', N'contacto@distribuidoracalzados.com')
INSERT [dbo].[Proveedores] ([ID_Proveedor], [Nombre_Proveedor], [Direccion], [Telefono], [Correo_Proveedor]) VALUES (4, N'Zapatos Elegantes', N'Blvd. Moda #101, Ciudad', N'555-3456', N'info@zapatoselegantes.com')
INSERT [dbo].[Proveedores] ([ID_Proveedor], [Nombre_Proveedor], [Direccion], [Telefono], [Correo_Proveedor]) VALUES (5, N'Calzado Internacional', N'Calle Global #202, Ciudad', N'555-7890', N'ventas@calzadointernacional.com')
INSERT [dbo].[Proveedores] ([ID_Proveedor], [Nombre_Proveedor], [Direccion], [Telefono], [Correo_Proveedor]) VALUES (6, N'Fábrica de Zapatos', N'Zona Industrial #303, Ciudad', N'555-1122', N'produccion@fabricazapatos.com')
INSERT [dbo].[Proveedores] ([ID_Proveedor], [Nombre_Proveedor], [Direccion], [Telefono], [Correo_Proveedor]) VALUES (7, N'Importaciones FootWear', N'Av. Comercial #404, Ciudad', N'555-3344', N'imports@footwear.com')
INSERT [dbo].[Proveedores] ([ID_Proveedor], [Nombre_Proveedor], [Direccion], [Telefono], [Correo_Proveedor]) VALUES (8, N'Distribuidora Paso Firme', N'Calle Segura #505, Ciudad', N'555-5566', N'ventas@pasofirme.com')
INSERT [dbo].[Proveedores] ([ID_Proveedor], [Nombre_Proveedor], [Direccion], [Telefono], [Correo_Proveedor]) VALUES (9, N'Cueros y Más', N'Av. Artesanal #606, Ciudad', N'555-7788', N'cueros@cuerosymas.com')
SET IDENTITY_INSERT [dbo].[Proveedores] OFF
GO
SET IDENTITY_INSERT [dbo].[Ventas] ON 

INSERT [dbo].[Ventas] ([ID_Venta], [Hora_Fecha], [Total_Precio], [ID_Empleado], [Metodo_Pago]) VALUES (1, CAST(N'2025-04-09T21:33:05.333' AS DateTime), CAST(0.00 AS Decimal(10, 2)), 1, N'Efectivo')
INSERT [dbo].[Ventas] ([ID_Venta], [Hora_Fecha], [Total_Precio], [ID_Empleado], [Metodo_Pago]) VALUES (2, CAST(N'2025-04-08T21:33:05.333' AS DateTime), CAST(0.00 AS Decimal(10, 2)), 2, N'Tarjeta de Crédito')
INSERT [dbo].[Ventas] ([ID_Venta], [Hora_Fecha], [Total_Precio], [ID_Empleado], [Metodo_Pago]) VALUES (3, CAST(N'2025-04-07T21:33:05.333' AS DateTime), CAST(0.00 AS Decimal(10, 2)), 3, N'Tarjeta de Débito')
INSERT [dbo].[Ventas] ([ID_Venta], [Hora_Fecha], [Total_Precio], [ID_Empleado], [Metodo_Pago]) VALUES (4, CAST(N'2025-04-05T21:33:05.333' AS DateTime), CAST(0.00 AS Decimal(10, 2)), 4, N'Efectivo')
INSERT [dbo].[Ventas] ([ID_Venta], [Hora_Fecha], [Total_Precio], [ID_Empleado], [Metodo_Pago]) VALUES (5, CAST(N'2025-04-03T21:33:05.333' AS DateTime), CAST(0.00 AS Decimal(10, 2)), 5, N'Transferencia')
INSERT [dbo].[Ventas] ([ID_Venta], [Hora_Fecha], [Total_Precio], [ID_Empleado], [Metodo_Pago]) VALUES (6, CAST(N'2025-03-31T21:33:05.333' AS DateTime), CAST(0.00 AS Decimal(10, 2)), 1, N'Tarjeta de Crédito')
INSERT [dbo].[Ventas] ([ID_Venta], [Hora_Fecha], [Total_Precio], [ID_Empleado], [Metodo_Pago]) VALUES (7, CAST(N'2025-03-29T21:33:05.333' AS DateTime), CAST(0.00 AS Decimal(10, 2)), 7, N'Efectivo')
INSERT [dbo].[Ventas] ([ID_Venta], [Hora_Fecha], [Total_Precio], [ID_Empleado], [Metodo_Pago]) VALUES (8, CAST(N'2025-03-26T21:33:05.333' AS DateTime), CAST(0.00 AS Decimal(10, 2)), 2, N'Tarjeta de Débito')
INSERT [dbo].[Ventas] ([ID_Venta], [Hora_Fecha], [Total_Precio], [ID_Empleado], [Metodo_Pago]) VALUES (9, CAST(N'2025-03-23T21:33:05.333' AS DateTime), CAST(0.00 AS Decimal(10, 2)), 9, N'Tarjeta de Crédito')
INSERT [dbo].[Ventas] ([ID_Venta], [Hora_Fecha], [Total_Precio], [ID_Empleado], [Metodo_Pago]) VALUES (10, CAST(N'2025-03-21T21:33:05.333' AS DateTime), CAST(0.00 AS Decimal(10, 2)), 3, N'Efectivo')
SET IDENTITY_INSERT [dbo].[Ventas] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Producto__7E43ED7C04B11AF2]    Script Date: 8/5/2025 20:20:50 ******/
ALTER TABLE [dbo].[Productos] ADD UNIQUE NONCLUSTERED 
(
	[Codigo_Barra] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Productos] ADD  DEFAULT ((0)) FOR [Stock]
GO
ALTER TABLE [dbo].[Ventas] ADD  DEFAULT (getdate()) FOR [Hora_Fecha]
GO
ALTER TABLE [dbo].[Detalle_Venta]  WITH CHECK ADD  CONSTRAINT [FK_DetalleVenta_Producto] FOREIGN KEY([ID_Producto])
REFERENCES [dbo].[Productos] ([ID_Producto])
GO
ALTER TABLE [dbo].[Detalle_Venta] CHECK CONSTRAINT [FK_DetalleVenta_Producto]
GO
ALTER TABLE [dbo].[Detalle_Venta]  WITH CHECK ADD  CONSTRAINT [FK_DetalleVenta_Venta] FOREIGN KEY([ID_Venta])
REFERENCES [dbo].[Ventas] ([ID_Venta])
GO
ALTER TABLE [dbo].[Detalle_Venta] CHECK CONSTRAINT [FK_DetalleVenta_Venta]
GO
ALTER TABLE [dbo].[Productos]  WITH CHECK ADD  CONSTRAINT [FK_Producto_Categoria] FOREIGN KEY([ID_Categoria])
REFERENCES [dbo].[Categoria] ([ID_Categoria])
GO
ALTER TABLE [dbo].[Productos] CHECK CONSTRAINT [FK_Producto_Categoria]
GO
ALTER TABLE [dbo].[Productos]  WITH CHECK ADD  CONSTRAINT [FK_Producto_Proveedor] FOREIGN KEY([ID_Proveedor])
REFERENCES [dbo].[Proveedores] ([ID_Proveedor])
GO
ALTER TABLE [dbo].[Productos] CHECK CONSTRAINT [FK_Producto_Proveedor]
GO
ALTER TABLE [dbo].[Ventas]  WITH CHECK ADD  CONSTRAINT [FK_Venta_Empleado] FOREIGN KEY([ID_Empleado])
REFERENCES [dbo].[Empleados] ([ID_Empleado])
GO
ALTER TABLE [dbo].[Ventas] CHECK CONSTRAINT [FK_Venta_Empleado]
GO
/****** Object:  StoredProcedure [dbo].[ActualizarCategoria]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[ActualizarCategoria]
    @ID_Categoria INT,
    @Nombre_Categoria VARCHAR(50)
AS
BEGIN
    UPDATE Categoria
    SET Nombre_Categoria = @Nombre_Categoria
    WHERE ID_Categoria = @ID_Categoria;
    
    IF @@ROWCOUNT > 0
        SELECT 'Categoría actualizada correctamente' AS Resultado;
    ELSE
        SELECT 'No se encontró la categoría con ID ' + CAST(@ID_Categoria AS VARCHAR) AS Resultado;
END;
GO
/****** Object:  StoredProcedure [dbo].[ActualizarDetalleVenta]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Procedimiento para actualizar un detalle de venta existente
CREATE   PROCEDURE [dbo].[ActualizarDetalleVenta]
    @ID_Detalle_Venta INT,
    @ID_Venta INT,
    @ID_Producto INT,
    @Cantidad INT,
    @Precio_Unitario DECIMAL(10, 2)
AS
BEGIN
    -- Obtener cantidad actual para ajustar el stock
    DECLARE @CantidadActual INT;
    SELECT @CantidadActual = Cantidad FROM Detalle_Venta WHERE ID_Detalle_Venta = @ID_Detalle_Venta;
    
    IF @CantidadActual IS NULL
        SELECT 'No se encontró el detalle de venta con ID ' + CAST(@ID_Detalle_Venta AS VARCHAR) AS Resultado;
    ELSE
    BEGIN
        -- Verificar si hay suficiente stock para el aumento en cantidad
        IF @Cantidad > @CantidadActual
        BEGIN
            DECLARE @StockDisponible INT, @DiferenciaStock INT;
            SET @DiferenciaStock = @Cantidad - @CantidadActual;
            SELECT @StockDisponible = Stock FROM Productos WHERE ID_Producto = @ID_Producto;
            
            IF @StockDisponible < @DiferenciaStock
                SELECT 'Error: No hay suficiente stock disponible para el aumento. Stock actual: ' + CAST(@StockDisponible AS VARCHAR) AS Resultado;
                RETURN;
        END
        
        -- Actualizar detalle de venta
        UPDATE Detalle_Venta
        SET ID_Venta = @ID_Venta,
            ID_Producto = @ID_Producto,
            Cantidad = @Cantidad,
            Precio_Unitario = @Precio_Unitario
        WHERE ID_Detalle_Venta = @ID_Detalle_Venta;
        
        -- Ajustar stock del producto
        UPDATE Productos
        SET Stock = CASE 
                     WHEN @Cantidad > @CantidadActual THEN Stock - (@Cantidad - @CantidadActual)
                     ELSE Stock + (@CantidadActual - @Cantidad)
                   END
        WHERE ID_Producto = @ID_Producto;
        
        -- Recalcular total de la venta
        UPDATE Ventas
        SET Total_Precio = (
            SELECT SUM(Cantidad * Precio_Unitario)
            FROM Detalle_Venta
            WHERE ID_Venta = @ID_Venta
        )
        WHERE ID_Venta = @ID_Venta;
        
        SELECT 'Detalle de venta actualizado correctamente' AS Resultado;
    END
END;
GO
/****** Object:  StoredProcedure [dbo].[ActualizarEmpleado]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Procedimiento para actualizar un empleado existente
CREATE   PROCEDURE [dbo].[ActualizarEmpleado]
    @ID_Empleado INT,
    @Nombre_Empleado VARCHAR(50),
    @Apellido_Empleado VARCHAR(50),
    @Direccion VARCHAR(200),
    @Telefono VARCHAR(20),
    @Correo VARCHAR(100),
    @Posicion VARCHAR(50),
    @Salario DECIMAL(10, 2)
AS
BEGIN
    UPDATE Empleados
    SET Nombre_Empleado = @Nombre_Empleado,
        Apellido_Empleado = @Apellido_Empleado,
        Direccion = @Direccion,
        Telefono = @Telefono,
        Correo = @Correo,
        Posicion = @Posicion,
        Salario = @Salario
    WHERE ID_Empleado = @ID_Empleado;
    
    IF @@ROWCOUNT > 0
        SELECT 'Empleado actualizado correctamente' AS Resultado;
    ELSE
        SELECT 'No se encontró el empleado con ID ' + CAST(@ID_Empleado AS VARCHAR) AS Resultado;
END;
GO
/****** Object:  StoredProcedure [dbo].[ActualizarProducto]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Procedimiento para actualizar un producto existente
CREATE   PROCEDURE [dbo].[ActualizarProducto]
    @ID_Producto INT,
    @Nombre_Producto VARCHAR(100),
    @Talla VARCHAR(10),
    @Modelo VARCHAR(50),
    @Marca VARCHAR(50),
    @Color VARCHAR(30),
    @Precio_Unitario DECIMAL(10, 2),
    @Material VARCHAR(50),
    @Stock INT,
    @ID_Categoria INT,
    @ID_Proveedor INT,
    @Imagen_Producto VARCHAR(200) = NULL,
    @Codigo_Barra VARCHAR(50)
AS
BEGIN
    UPDATE Productos
    SET Nombre_Producto = @Nombre_Producto,
        Talla = @Talla,
        Modelo = @Modelo,
        Marca = @Marca,
        Color = @Color,
        Precio_Unitario = @Precio_Unitario,
        Material = @Material,
        Stock = @Stock,
        ID_Categoria = @ID_Categoria,
        ID_Proveedor = @ID_Proveedor,
        Imagen_Producto = @Imagen_Producto,
        Codigo_Barra = @Codigo_Barra
    WHERE ID_Producto = @ID_Producto;
    
    IF @@ROWCOUNT > 0
        SELECT 'Producto actualizado correctamente' AS Resultado;
    ELSE
        SELECT 'No se encontró el producto con ID ' + CAST(@ID_Producto AS VARCHAR) AS Resultado;
END;
GO
/****** Object:  StoredProcedure [dbo].[ActualizarProveedor]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Procedimiento para actualizar un proveedor existente
CREATE   PROCEDURE [dbo].[ActualizarProveedor]
    @ID_Proveedor INT,
    @Nombre_Proveedor VARCHAR(100),
    @Direccion VARCHAR(200),
    @Telefono VARCHAR(20),
    @Correo_Proveedor VARCHAR(100)
AS
BEGIN
    UPDATE Proveedores
    SET Nombre_Proveedor = @Nombre_Proveedor,
        Direccion = @Direccion,
        Telefono = @Telefono,
        Correo_Proveedor = @Correo_Proveedor
    WHERE ID_Proveedor = @ID_Proveedor;
    
    IF @@ROWCOUNT > 0
        SELECT 'Proveedor actualizado correctamente' AS Resultado;
    ELSE
        SELECT 'No se encontró el proveedor con ID ' + CAST(@ID_Proveedor AS VARCHAR) AS Resultado;
END;
GO
/****** Object:  StoredProcedure [dbo].[ActualizarVenta]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Procedimiento para actualizar una venta existente
CREATE   PROCEDURE [dbo].[ActualizarVenta]
    @ID_Venta INT,
    @Total_Precio DECIMAL(10, 2),
    @ID_Empleado INT,
    @Metodo_Pago VARCHAR(50)
AS
BEGIN
    UPDATE Ventas
    SET Total_Precio = @Total_Precio,
        ID_Empleado = @ID_Empleado,
        Metodo_Pago = @Metodo_Pago
    WHERE ID_Venta = @ID_Venta;
    
    IF @@ROWCOUNT > 0
        SELECT 'Venta actualizada correctamente' AS Resultado;
    ELSE
        SELECT 'No se encontró la venta con ID ' + CAST(@ID_Venta AS VARCHAR) AS Resultado;
END;
GO
/****** Object:  StoredProcedure [dbo].[BuscarCategoria]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Procedimiento para buscar categorías por nombre (adivinar)
CREATE   PROCEDURE [dbo].[BuscarCategoria]
    @Patron_Nombre VARCHAR(50)
AS
BEGIN
    SELECT ID_Categoria, Nombre_Categoria
    FROM Categoria
    WHERE Nombre_Categoria LIKE '%' + @Patron_Nombre + '%'
    ORDER BY Nombre_Categoria;
END;
GO
/****** Object:  StoredProcedure [dbo].[BuscarDetalleVenta]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Procedimiento para buscar detalles de venta por ID de venta o producto (adivinar)
CREATE   PROCEDURE [dbo].[BuscarDetalleVenta]
    @ID_Venta INT = NULL,
    @ID_Producto INT = NULL
AS
BEGIN
    SELECT dv.ID_Detalle_Venta, dv.ID_Venta, dv.ID_Producto, dv.Cantidad, dv.Precio_Unitario,
           p.Nombre_Producto, p.Modelo, p.Marca, p.Color, p.Talla,
           (dv.Cantidad * dv.Precio_Unitario) AS Subtotal
    FROM Detalle_Venta dv
    INNER JOIN Productos p ON dv.ID_Producto = p.ID_Producto
    WHERE (@ID_Venta IS NULL OR dv.ID_Venta = @ID_Venta)
      AND (@ID_Producto IS NULL OR dv.ID_Producto = @ID_Producto)
    ORDER BY dv.ID_Venta, dv.ID_Detalle_Venta;
END;
GO
/****** Object:  StoredProcedure [dbo].[BuscarEmpleado]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Procedimiento para buscar empleados por nombre, apellido o posición (adivinar)
CREATE   PROCEDURE [dbo].[BuscarEmpleado]
    @Patron VARCHAR(50)
AS
BEGIN
    SELECT ID_Empleado, Nombre_Empleado, Apellido_Empleado, Posicion, Salario, Telefono, Correo
    FROM Empleados
    WHERE Nombre_Empleado LIKE '%' + @Patron + '%'
       OR Apellido_Empleado LIKE '%' + @Patron + '%'
       OR Posicion LIKE '%' + @Patron + '%'
    ORDER BY Apellido_Empleado, Nombre_Empleado;
END;
GO
/****** Object:  StoredProcedure [dbo].[BuscarProducto]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Procedimiento para buscar productos por nombre, marca, modelo o color (adivinar)
CREATE   PROCEDURE [dbo].[BuscarProducto]
    @Patron VARCHAR(100)
AS
BEGIN
    SELECT p.ID_Producto, p.Nombre_Producto, p.Talla, p.Modelo, p.Marca, p.Color, 
           p.Precio_Unitario, p.Stock, c.Nombre_Categoria, prov.Nombre_Proveedor
    FROM Productos p
    INNER JOIN Categoria c ON p.ID_Categoria = c.ID_Categoria
    INNER JOIN Proveedores prov ON p.ID_Proveedor = prov.ID_Proveedor
    WHERE p.Nombre_Producto LIKE '%' + @Patron + '%'
       OR p.Marca LIKE '%' + @Patron + '%'
       OR p.Modelo LIKE '%' + @Patron + '%'
       OR p.Color LIKE '%' + @Patron + '%'
       OR p.Codigo_Barra LIKE '%' + @Patron + '%'
    ORDER BY p.Nombre_Producto;
END;
GO
/****** Object:  StoredProcedure [dbo].[BuscarProveedor]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Procedimiento para buscar proveedores por nombre o teléfono (adivinar)
CREATE   PROCEDURE [dbo].[BuscarProveedor]
    @Patron VARCHAR(100)
AS
BEGIN
    SELECT ID_Proveedor, Nombre_Proveedor, Direccion, Telefono, Correo_Proveedor
    FROM Proveedores
    WHERE Nombre_Proveedor LIKE '%' + @Patron + '%'
       OR Telefono LIKE '%' + @Patron + '%'
    ORDER BY Nombre_Proveedor;
END;
GO
/****** Object:  StoredProcedure [dbo].[BuscarVenta]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Procedimiento para buscar ventas por fecha o empleado (adivinar)
CREATE   PROCEDURE [dbo].[BuscarVenta]
    @Fecha_Inicio DATETIME = NULL,
    @Fecha_Fin DATETIME = NULL,
    @ID_Empleado INT = NULL
AS
BEGIN
    SELECT v.ID_Venta, v.Hora_Fecha, v.Total_Precio, v.Metodo_Pago,
           e.Nombre_Empleado + ' ' + e.Apellido_Empleado AS Nombre_Completo
    FROM Ventas v
    INNER JOIN Empleados e ON v.ID_Empleado = e.ID_Empleado
    WHERE (@Fecha_Inicio IS NULL OR v.Hora_Fecha >= @Fecha_Inicio)
      AND (@Fecha_Fin IS NULL OR v.Hora_Fecha <= @Fecha_Fin)
      AND (@ID_Empleado IS NULL OR v.ID_Empleado = @ID_Empleado)
    ORDER BY v.Hora_Fecha DESC;
END;
GO
/****** Object:  StoredProcedure [dbo].[InsertarCategoria]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[InsertarCategoria]
    @Nombre_Categoria VARCHAR(50)
AS
BEGIN
    INSERT INTO Categoria (Nombre_Categoria)
    VALUES (@Nombre_Categoria);
    
    SELECT SCOPE_IDENTITY() AS ID_Categoria;
END;
GO
/****** Object:  StoredProcedure [dbo].[InsertarDetalleVenta]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ---------------------------------------------
-- Procedimientos para la tabla DETALLE_VENTA
-- ----------------------------------------------

-- Procedimiento para insertar un nuevo detalle de venta
CREATE   PROCEDURE [dbo].[InsertarDetalleVenta]
    @ID_Venta INT,
    @ID_Producto INT,
    @Cantidad INT,
    @Precio_Unitario DECIMAL(10, 2)
AS
BEGIN
    -- Verificar stock disponible
    DECLARE @StockDisponible INT;
    SELECT @StockDisponible = Stock FROM Productos WHERE ID_Producto = @ID_Producto;
    
    IF @StockDisponible < @Cantidad
        SELECT 'Error: No hay suficiente stock disponible. Stock actual: ' + CAST(@StockDisponible AS VARCHAR) AS Resultado;
    ELSE
    BEGIN
        -- Insertar detalle de venta
        INSERT INTO Detalle_Venta (ID_Venta, ID_Producto, Cantidad, Precio_Unitario)
        VALUES (@ID_Venta, @ID_Producto, @Cantidad, @Precio_Unitario);
        
        -- Actualizar stock del producto
        UPDATE Productos
        SET Stock = Stock - @Cantidad
        WHERE ID_Producto = @ID_Producto;
        
        -- Actualizar el total de la venta
        UPDATE Ventas
        SET Total_Precio = Total_Precio + (@Precio_Unitario * @Cantidad)
        WHERE ID_Venta = @ID_Venta;
        
        SELECT SCOPE_IDENTITY() AS ID_Detalle_Venta, 'Detalle de venta añadido correctamente' AS Resultado;
    END
END;
GO
/****** Object:  StoredProcedure [dbo].[InsertarEmpleado]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ------------------------------------------
-- Procedimientos para la tabla EMPLEADOS
-- ------------------------------------------

-- Procedimiento para insertar un nuevo empleado
CREATE   PROCEDURE [dbo].[InsertarEmpleado]
    @Nombre_Empleado VARCHAR(50),
    @Apellido_Empleado VARCHAR(50),
    @Direccion VARCHAR(200),
    @Telefono VARCHAR(20),
    @Correo VARCHAR(100),
    @Posicion VARCHAR(50),
    @Salario DECIMAL(10, 2)
AS
BEGIN
    INSERT INTO Empleados (Nombre_Empleado, Apellido_Empleado, Direccion, Telefono, Correo, Posicion, Salario)
    VALUES (@Nombre_Empleado, @Apellido_Empleado, @Direccion, @Telefono, @Correo, @Posicion, @Salario);
    
    SELECT SCOPE_IDENTITY() AS ID_Empleado;
END;
GO
/****** Object:  StoredProcedure [dbo].[InsertarProducto]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ----------------------------------------------
-- Procedimientos para la tabla PRODUCTOS
-- ---------------------------------------------

-- Procedimiento para insertar un nuevo producto
CREATE   PROCEDURE [dbo].[InsertarProducto]
    @Nombre_Producto VARCHAR(100),
    @Talla VARCHAR(10),
    @Modelo VARCHAR(50),
    @Marca VARCHAR(50),
    @Color VARCHAR(30),
    @Precio_Unitario DECIMAL(10, 2),
    @Material VARCHAR(50),
    @Stock INT,
    @ID_Categoria INT,
    @ID_Proveedor INT,
    @Imagen_Producto VARCHAR(200) = NULL,
    @Codigo_Barra VARCHAR(50)
AS
BEGIN
    INSERT INTO Productos (Nombre_Producto, Talla, Modelo, Marca, Color, Precio_Unitario, 
                         Material, Stock, ID_Categoria, ID_Proveedor, Imagen_Producto, Codigo_Barra)
    VALUES (@Nombre_Producto, @Talla, @Modelo, @Marca, @Color, @Precio_Unitario, 
            @Material, @Stock, @ID_Categoria, @ID_Proveedor, @Imagen_Producto, @Codigo_Barra);
    
    SELECT SCOPE_IDENTITY() AS ID_Producto;
END;
GO
/****** Object:  StoredProcedure [dbo].[InsertarProveedor]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ------------------------------------------
-- Procedimientos para la tabla PROVEEDORES
-- ------------------------------------------

-- Procedimiento para insertar un nuevo proveedor
CREATE   PROCEDURE [dbo].[InsertarProveedor]
    @Nombre_Proveedor VARCHAR(100),
    @Direccion VARCHAR(200),
    @Telefono VARCHAR(20),
    @Correo_Proveedor VARCHAR(100)
AS
BEGIN
    INSERT INTO Proveedores (Nombre_Proveedor, Direccion, Telefono, Correo_Proveedor)
    VALUES (@Nombre_Proveedor, @Direccion, @Telefono, @Correo_Proveedor);
    
    SELECT SCOPE_IDENTITY() AS ID_Proveedor;
END;
GO
/****** Object:  StoredProcedure [dbo].[InsertarVenta]    Script Date: 8/5/2025 20:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- --------------------------------------------
-- Procedimientos para la tabla VENTAS
-- -------------------------------------------

-- Procedimiento para insertar una nueva venta
CREATE   PROCEDURE [dbo].[InsertarVenta]
    @ID_Empleado INT,
    @Metodo_Pago VARCHAR(50),
    @Total_Precio DECIMAL(10, 2) = 0
AS
BEGIN
    INSERT INTO Ventas (Hora_Fecha, Total_Precio, ID_Empleado, Metodo_Pago)
    VALUES (GETDATE(), @Total_Precio, @ID_Empleado, @Metodo_Pago);
    
    SELECT SCOPE_IDENTITY() AS ID_Venta;
END;
GO
USE [master]
GO
ALTER DATABASE [ZapateriaBD] SET  READ_WRITE 
GO
