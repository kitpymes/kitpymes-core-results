# <img src="https://github.com/kitpymes/kitpymes-core-results/raw/master/docs/images/logo.png" height="30px"> Kitpymes.Core.Results

**Extensiones y utilidades comunes compartidas en los proyectos**

[![Build Status](https://github.com/kitpymes/kitpymes-core-results/actions/workflows/ci.yml/badge.svg)](https://github.com/kitpymes/kitpymes-core-results/actions/workflows/ci.yml)
[![NuGet Status](https://img.shields.io/nuget/v/Kitpymes.Core.Results)](https://www.nuget.org/packages/Kitpymes.Core.Results/)
[![NuGet Download](https://img.shields.io/nuget/dt/Kitpymes.Core.Results)](https://www.nuget.org/stats/packages/Kitpymes.Core.Results?groupby=Version)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/kitpymes/kitpymes-core-results/blob/master/docs/LICENSE.txt)
[![Size Repo](https://img.shields.io/github/repo-size/kitpymes/kitpymes-core-results)](https://github.com/kitpymes/kitpymes-core-results/)
[![Last Commit](https://img.shields.io/github/last-commit/kitpymes/kitpymes-core-results)](https://github.com/kitpymes/kitpymes-core-results/)

## 📋 Requerimientos 

* Visual Studio = 2022

* NET TargetFramework = net8.0

* Net Core SDK = 8.0.x

* C# = 12

* Conocimientos sobre Inyección de Dependencias

## 🔧 Instalación 

_Se puede instalar usando el administrador de paquetes Nuget o CLI dotnet._

_Nuget_

```
Install-Package Kitpymes.Core.Results
```

_CLI dotnet_

```
dotnet add package Kitpymes.Core.Results
```

## ⌨️ Código


### Extensions

```cs
public class ApiResult : IApiResult
{
    public static ApiResult Ok() {}

    public static ApiResult Ok(Action<ResultOptionsOk> options) {}

    public static ApiResult BadRequest(string message) {}

    public static ApiResult BadRequest(string message, object details) {}

    public static ApiResult BadRequest(IEnumerable<string> messages) {}

    public static ApiResult BadRequest(IDictionary<string, IEnumerable<string>> errors) {}

    public static ApiResult BadRequest(IEnumerable<(string fieldName, string message)> errors) {}

    public static ApiResult Error(Action<ResultOptionsError> options) {}

```

## ⚙️ Pruebas Unitarias

_Cada proyecto tiene su test que se ejecutan desde el "Explorador de pruebas"_

![Resultado pruebas](images/screenshot/resultados_testing.png)


## 🛠️ Construido con 

* [NET Core](https://dotnet.microsoft.com/download) - Framework de trabajo
* [C#](https://docs.microsoft.com/es-es/dotnet/csharp/) - Lenguaje de programación
* [Inserción de dependencias](https://docs.microsoft.com/es-es/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.0) - Patrón de diseño de software
* [MSTest](https://docs.microsoft.com/es-es/dotnet/core/testing/unit-testing-with-mstest) - Pruebas unitarias
* [Nuget](https://www.nuget.org/) - Manejador de dependencias
* [Visual Studio](https://visualstudio.microsoft.com/) - Entorno de programacion


## ✒️ Autores 

* **Sebastian R Ferrari** - *Trabajo Inicial* - [kitpymes](https://kitpymes.com)


## 📄 Licencia 

* Este proyecto está bajo la Licencia [LICENSE](LICENSE.txt)


## 🎁 Gratitud 

* Este proyecto fue diseñado para compartir, creemos que es la mejor forma de ayudar 📢
* Cada persona que contribuya sera invitada a tomar una 🍺 
* Gracias a todos! 🤓

---
[Kitpymes](https://github.com/kitpymes) 😊