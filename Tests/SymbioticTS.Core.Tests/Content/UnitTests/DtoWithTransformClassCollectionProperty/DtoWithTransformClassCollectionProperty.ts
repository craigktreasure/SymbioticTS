﻿/**
 * This file was generated by SymbioticTS.
 * All changes will be lost the next time the file is generated.
 */

/* tslint:disable:max-line-length */

import { ClassRequiresTransform } from './ClassRequiresTransform';
import { IDtoWithTransformClassCollectionPropertyDto } from './IDtoWithTransformClassCollectionPropertyDto';

export class DtoWithTransformClassCollectionProperty {
    public transform: ClassRequiresTransform[];

    constructor(transform: ClassRequiresTransform[]) {
        this.transform = transform;
    }

    public static fromDto(dto: IDtoWithTransformClassCollectionPropertyDto): DtoWithTransformClassCollectionProperty {
        const transform = dto.transform.map(ClassRequiresTransform.fromDto);

        return new DtoWithTransformClassCollectionProperty(transform);
    }
}
