﻿/**
 * This file was generated by SymbioticTS.
 * All changes will be lost the next time the file is generated.
 */

// tslint:disable:max-line-length

import { IBaseShapeDto } from './IBaseShapeDto';
import { IQuadrilateralDto } from './IQuadrilateralDto';
import { IShapeDto } from './IShapeDto';

export interface IRectangleDto extends IBaseShapeDto, IShapeDto, IQuadrilateralDto {
    readonly height: number;
    readonly width: number;
}