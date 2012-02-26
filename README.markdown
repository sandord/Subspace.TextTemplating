Subspace.TextTemplating is a .NET text templating library.

**Key features**

* Allows for creating text based output dynamically by using templates with inline code;
* Largely T4 compatible;
* Supports the C# language;
* Works in a fashion similar to popular web oriented server side languages such as the ASP.NET family and PHP;
* Supports the Visual Studio debugger allowing you to step through the template file;
* Supports template properties, which provide access to existing object instances from within a template.

**Some benefits over T4**

* Supports runtime transformation of runtime acquired templates;
* Does not depend on Visual Studio so no need to ship additional copyrighted DLLs;
* Supports dynamic parsing of templates from inline code, allowing for conditional template includes.

**Some of the possible uses are**

* Store e-mail templates in a database and transform them on the fly;
* Change text template files without having to recompile your application.

**Examples**

You can take a look at the supplied unit tests for some usage examples.
