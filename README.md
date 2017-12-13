# Felinesorter Application
Consumes a JSON webservice and displays the cats according to the gender of their owner

[![Build status](https://ci.appveyor.com/api/projects/status/qnnt1f8rakhpukcu?svg=true)](https://ci.appveyor.com/project/ruskindantra/felinesorter) [![License](http://img.shields.io/:license-mit-blue.svg)](https://raw.githubusercontent.com/ruskindantra/felinesorter/master/LICENSE)

## Demonstrated Principles
### SOLID Programming
1. Each class is responsible for a single piece of functionality all the way down to it's methods and variables
1. Most classes are marked as internal so they cannot be used outside the library.  The interfaces are left open so that they can be implemented in a different way if required.  Good example is the `IOwnerSorter` interface.
1. As all classes are constructed using interfaces the Liskov's substitution principle can be exercised if required
1. I have an interface for each functionality rather than one interface which does it all
1. Dependency injection is implemented using the Autofac dependency container allowing flexibility to plug and play different implementations if required

### Logging
1. Logging is done to a file using SeriLog *(very nice and simple)*
1. Console logging is commented out to emphasize the output of the application

### Settings
1. An `AppSettings.json` file is provided to configure the endpoints and logging levels as required during runtime

### Unit testing
1. Implemented using XUnit
1. Interfaces mocked using Moq and AutoMoq
1. Assertions done using FluentAssertions
1. Unit tests are placed in files matching the files they are testing in the same folder structure as the project they are testing
1. Unit tests are named exactly as their classes except with underscore and all lower case *(this is intentional as it allows for quick searching with tools like Resharper, debateable :)*
