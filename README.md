# SymbioticTS

[![Build Status](https://treasure.visualstudio.com/SymbioticTS/_apis/build/status/craigktreasure.SymbioticTS)](https://treasure.visualstudio.com/SymbioticTS/_build/latest?definitionId=13)

## Status: Planning

This project is not yet "real" and is currently being planned and developed.

## Goals

Make consuming objects defined in a .NET backend eaiser to consume from TypeScript code.

As you pass objects to a frontend in json format, you lose some type fidelity. For example, if you have an object that has a property that is a `DateTime`, that object will be serialized to a `string`. Many projects similar to this would either generate a type definition that says the object is either a `Date`, `string`, or `any`. The truth is, after the json is parsed, it is a `string`. But, you want it to be parsed as a `Date`. This project will provide types that represent the true json that can then be converted to another object that is meant to be consumed by the rest of your application with the types that better map to your original .NET type. This prevents bugs and handles geneating the boilerplate code for you.

## Features

* Generates DTO helper objects with support for handling `DateTime`.
* Inferred accessors and types (readonly, optional, etc.).
* Support for inheritance.
* Auto-generate supporting objects like enums, interfaces, and classes.

## Attributes

* TsDto
  * Generates a TypeScript DTO interface and class with an adapter function on the class.
* TsInterface
  * Explicitly marks an interface to be generated.
* TsClass
  * Explicitly marks a class to be generated.
* TsProperty
  * Used to explicitly configure a property: readonly, optional, etc.

## Workflow

* Add the SymbioticTypeScript.MSBuild NuGet package to a project.
* Configure output location.
* Attribute an interface with `TsInterface` or a class with `TsDto` or `TsClass`.
* Build
  * Discover the full closure of the built assembly dependencies (except for .NET assemblies).
  * Discover public or internal attributed classes and interfaces.
  * Generate TypeScript objects for attributed and supporting .NET objects.

## Examples

### .NET Class

``` CSharp
[TsDto]
internal class Rectangle
{
    public int X { get; set; }
    public int Y { get; set; }
    public DateTime DateCreated { get; }
}
```

### Generated TypeScript objects (proposed)

``` TypeScript
export interface IRectangleDto
{
    x: number;
    y: number;
    readonly dateCreated: string;
}

export class RectangleDto
{
    private readonly _dateCreated: Date;

    public get dateCreated(): Date {
        return this._dateCreated;
    }

    constructor(public x: number, public y: number, dateCreated: Date) {
        this._dateCreated = dateCreated;
    }

    public static fromDto(dto: IRectangleDto): Rectangle
    {
        const x = dto.x;
        const y = dto.y;
        const dateCreated = new Date(Date.parse(dto.dateCreated))

        return new Rectangle(x, y, dateCreated);
    }
}
```