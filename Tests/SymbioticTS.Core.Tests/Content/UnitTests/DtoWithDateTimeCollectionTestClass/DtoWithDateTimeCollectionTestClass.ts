﻿/**
 * This file was generated by SymbioticTS.
 * All changes will be lost the next time the file is generated.
 */

/* tslint:disable:max-line-length */

import { IDtoWithDateTimeCollectionTestClassDto } from './IDtoWithDateTimeCollectionTestClassDto';

export class DtoWithDateTimeCollectionTestClass {
    public count: number;
    public birthdate: Date[];

    constructor(birthdate: Date[], count: number) {
        this.birthdate = birthdate;
        this.count = count;
    }

    public static fromDto(dto: IDtoWithDateTimeCollectionTestClassDto): DtoWithDateTimeCollectionTestClass {
        const birthdate = dto.birthdate.map(s => new Date(s));

        return new DtoWithDateTimeCollectionTestClass(birthdate, dto.count);
    }
}
