# Felinesorter Application
Consumes a JSON webservice and displays the cats according to the gender of their owner

## Demonstrated Principles
### SOLID Programming
1. Each class is responsible for a single piece of functionality all the way down to it's methods and variables
1. Most classes are marked as internal so they cannot be used outside the library.  The interfaces are left open so that they can be implemented in a different way if required.  Good example is the `IOwnerSorter` interface.
1. As all classes are constructed using interfaces the Liskov's substitution principle can be exercised if required
1. I have an interface for each functionality rather than one interface which does it all
1. Dependency injection is implemented using the Autofac dependency container allowing flexibility to plug and play different implementations if required

### Unit testing
