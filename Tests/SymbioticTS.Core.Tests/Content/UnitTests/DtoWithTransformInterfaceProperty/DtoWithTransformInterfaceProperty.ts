﻿/**
 * This file was generated by SymbioticTS.
 * All changes will be lost the next time the file is generated.
 */

/* tslint:disable:max-line-length */

import { IDtoWithTransformInterfacePropertyDto } from './IDtoWithTransformInterfacePropertyDto';
import { IRequireTransform } from './IRequireTransform';
import { IRequireTransformDto } from './IRequireTransformDto';

export class DtoWithTransformInterfaceProperty {
    public property: IRequireTransform;

    constructor(property: IRequireTransform) {
        this.property = property;
    }

    public static fromDto(dto: IDtoWithTransformInterfacePropertyDto): DtoWithTransformInterfaceProperty {
        const property = DtoWithTransformInterfaceProperty.fromIRequireTransformDto(dto.property);

        return new DtoWithTransformInterfaceProperty(property);
    }

    private static fromIRequireTransformDto(dto: IRequireTransformDto): IRequireTransform {
        const birthdate = new Date(dto.birthdate);

        return {
            birthdate: birthdate
        };
    }
}
