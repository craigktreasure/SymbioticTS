﻿/**
 * This file was generated by SymbioticTS.
 * All changes will be lost the next time the file is generated.
 */

/* tslint:disable:max-line-length */

import { UnnanotatedGenericClass } from './UnnanotatedGenericClass';
import { UnnanotatedNullableGenericStruct } from './UnnanotatedNullableGenericStruct';

export class ClassWithUnannotatedGenericProperties {
    public genericClassProperty: UnnanotatedGenericClass[];
    public nullableStructProperty?: UnnanotatedNullableGenericStruct;

    constructor(genericClassProperty: UnnanotatedGenericClass[], nullableStructProperty?: UnnanotatedNullableGenericStruct) {
        this.genericClassProperty = genericClassProperty;
        this.nullableStructProperty = nullableStructProperty;
    }
}
