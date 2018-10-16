﻿/**
 * This file was generated by SymbioticTS.
 * All changes will be lost the next time the file is generated.
 */

import { IDtoWithTransformInterfaceCollectionPropertyDto } from './IDtoWithTransformInterfaceCollectionPropertyDto';
import { IRequireTransform } from './IRequireTransform';
import { IRequireTransformDto } from './IRequireTransformDto';

export class DtoWithTransformInterfaceCollectionProperty
{
    public property?: IRequireTransform[];

    constructor(property?: IRequireTransform[])
    {
        this.property = property;
    }

    public static fromDto(dto: IDtoWithTransformInterfaceCollectionPropertyDto): DtoWithTransformInterfaceCollectionProperty
    {
        const property = dto.property === undefined ? undefined : dto.property.map(DtoWithTransformInterfaceCollectionProperty.fromIRequireTransformDto);

        return new DtoWithTransformInterfaceCollectionProperty(property);
    }

    private static fromIRequireTransformDto(dto: IRequireTransformDto): IRequireTransform
    {
        const birthdate = new Date(dto.birthdate);

        return {
            birthdate: birthdate
        };
    }
}
