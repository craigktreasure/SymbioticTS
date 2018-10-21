﻿/**
 * This file was generated by SymbioticTS.
 * All changes will be lost the next time the file is generated.
 */

// tslint:disable:max-line-length

import { ClassRequiresTransform } from './ClassRequiresTransform';
import { IDtoWithTransformClassPropertyDto } from './IDtoWithTransformClassPropertyDto';

export class DtoWithTransformClassProperty {
    public transform?: ClassRequiresTransform;

    constructor(transform?: ClassRequiresTransform) {
        this.transform = transform;
    }

    public static fromDto(dto: IDtoWithTransformClassPropertyDto): DtoWithTransformClassProperty {
        const transform = dto.transform === undefined ? undefined : ClassRequiresTransform.fromDto(dto.transform);

        return new DtoWithTransformClassProperty(transform);
    }
}