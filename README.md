# TypeSupport
[![nuget](https://img.shields.io/nuget/v/TypeSupport.svg)](https://www.nuget.org/packages/TypeSupport/)
[![nuget](https://img.shields.io/nuget/dt/TypeSupport.svg)](https://www.nuget.org/packages/TypeSupport/)
[![Build status](https://ci.appveyor.com/api/projects/status/swv5vcad12nrwohk?svg=true)](https://ci.appveyor.com/project/MichaelBrown/typesupport)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/5ddab4815a2a49b3babac9af232f9f04)](https://www.codacy.com/app/replaysMike/TypeSupport?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=replaysMike/TypeSupport&amp;utm_campaign=Badge_Grade)
[![Codacy Badge](https://api.codacy.com/project/badge/Coverage/5ddab4815a2a49b3babac9af232f9f04)](https://www.codacy.com/app/replaysMike/TypeSupport?utm_source=github.com&utm_medium=referral&utm_content=replaysMike/TypeSupport&utm_campaign=Badge_Coverage)

A CSharp library that makes it easier to work with Types dynamically. TypeSupport includes a flexible Object factory for creating and initializing all kinds of types.

## Description

The best way to understand what TypeSupport can do is to see it in action! It is used as the foundation for many other packages.

## Installation
Install TypeSupport from the Package Manager Console:
```PowerShell
PM> Install-Package TypeSupport
```

## Usage

### Type Support

Getting started - create a TypeSupport from a type
```csharp
using TypeSupport;

var type = typeof(MyObject);
var typeSupport = new ExtendedType(type);
```

or do it using the extensions (we will use this syntax going forward):
```csharp
using TypeSupport;

var type = typeof(MyObject);
var typeSupport = type.GetExtendedType();
```

get information about an array:
```csharp
var type = typeof(int[]);
var typeSupport = type.GetExtendedType();

var isArray = typeSupport.IsArray; // true
var elementType = typeSupport.ElementType; // int
```

get information about a Dictionary:
```csharp
var type = typeof(Dictionary<int, string>);
var typeSupport = type.GetExtendedType();

var isArray = typeSupport.IsDictionary; // true
var elementTypes = typeSupport.GenericArgumentTypes; // System.Int32, System.String
```

get info about an interface:
```csharp
var type = typeof(IVehicle);
var typeSupport = type.GetExtendedType();

var isArray = typeSupport.IsInterface; // true
var classesThatImplementICustomInterface = typeSupport.KnownConcreteTypes;
// [] = Car, Truck, Van, Motorcycle
```

get info about a class:
```csharp
[Description("A car object")]
public class Car : IVehicle
{
  public string Name { get; set; }
  public Car() { }
}
var type = typeof(Car);
var typeSupport = type.GetExtendedType();

var isArray = typeSupport.HasEmptyConstructor; // true
var attributes = typeSupport.Attributes;
// [] = DescriptionAttribute
```

working with enums:
```csharp
public enum Colors : byte
{
  Red = 1,
  Green = 2,
  Blue = 3
}
var type = typeof(Colors);
var typeSupport = type.GetExtendedType();

var isEnum = typeSupport.IsEnum; // true
var enumValues = typeSupport.EnumValues;
// [] = <1, Red>, <2, Green>, <3, blue>
var enumType = typeSupport.EnumType; // System.Byte
```

working with Tuples:
```csharp
var tupleType = typeof(Tuple<int, string, double>);
var valueTupleType = typeof((IVehicle, string));
var tupleTypeSupport = type.GetExtendedType();
var valueTupleTypeSupport = valueTupleType.GetExtendedType();

var isTuple = tupleTypeSupport.IsTuple; // true
var isValueTuple = valueTupleTypeSupport.IsValueTuple; // true
var tupleGenericArguments = tupleTypeSupport.GenericArgumentTypes; // System.Int32, System.String, System.Double
var valueTupleGenericArguments = valueTupleTypeSupport.GenericArgumentTypes; // IVehicle, System.String
// there's lots more you can do, such as getting the value from a Tuple instance:

var car = new Car();
var description = "My cool car";
var myTuple = (car, description);
var items = myTuple.GetValueTupleItemObjects();
// [] = Car, "My cool car"
```

### Object factory

Create new objects of any type:

```csharp
var factory = new ObjectFactory();
var listInstance = factory.CreateEmptyObject<IList<int>>(); // List<int>() 0 elements
var dictionaryInstance = factory.CreateEmptyObject<IDictionary<int, string>>(); // Dictionary<int, string>() 0 elements
var emptyByteArray = factory.CreateEmptyObject<byte[]>(); // byte[0] empty byte array
var byteArray = factory.CreateEmptyObject<byte[]>(length: 64); // byte[64]
var tupleInstance = factory.CreateEmptyObject<(int, string)>(); // tupleInstance.Item1 = 0, tupleInstance.item2 = null
var myComplexObject = factory.CreateEmptyObject<MyComplexObject>();
```

Create objects without parameterless constructors:
```csharp
public class CustomObject
{
  public int Id { get; }
  public CustomObject(int id)
  {
    Id = id;
  }
}
var factory = new ObjectFactory();
var myObj = factory.CreateEmptyObject<CustomObject>(); // myObj.GetType() == typeof(CustomObject)
```

You can instruct the Object factory on how to map abstract interfaces when creating instances:
```csharp
var typeRegistry = TypeRegistry.Configure((config) => {
  config.AddMapping<IVehicle, Car>();
});

var factory = new ObjectFactory(typeRegistry);
var car = factory.CreateEmptyObject<IVehicle>(); // car.GetType() == typeof(Car)
```

You can also register custom factories:
```csharp
var typeRegistry = TypeRegistry.Configure((config) => {
  config.AddFactory<IVehicle, Car>(() => new Car(Color.Red));
});

var factory = new ObjectFactory(typeRegistry);
var car = factory.CreateEmptyObject<IVehicle>(); // car.GetType() == typeof(Car)
```

### Capabilities

- [x] All basic types, enums, generics, collections and enumerables
- [x] Internal caching of type examination
- [x] Constructor analysis (empty constructors, parameterized constructors)
- [x] Easy listing of valid Enum values
- [x] Easy listing of concrete types implementing an interface
- [x] Easy listing of attributes
- [x] Easy listing of generic arguments
- [x] Easy listing of properties/fields
- [x] Easy listing of implemented interfaces
- [x] Easy listing of Tuple/ValueTuple types
- [x] Nullable type detection
- [x] Custom collection information detection
- [x] Primitive / Struct detection
- [x] Anonymous type detection
- [x] High performance testing and optimization
