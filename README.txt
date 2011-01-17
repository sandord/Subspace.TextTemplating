Subspace.TextTemplating is a .NET text templating library. It has the following features:

* Supports the C# language
* Partially T4 compatible
* Supports runtime transformation as opposed to T4
* Does not depend on Visual Studio as opposed to T4
* Supports the Visual Studio debugger by providing references to the template from the generated code
* Supports template properties, which provide access to existing instances from within templates
* Supports dynamic parsing of templates from inline code, allowing for conditional template includes

Some of the possible uses are:

* Store e-mail templates in a database and transform them on the fly
* Change text template files without recompiling your application

You can take a look at the supplied unit tests for some usage examples.
