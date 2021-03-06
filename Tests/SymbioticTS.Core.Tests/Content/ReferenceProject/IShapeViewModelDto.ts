﻿/**
 * This file was generated by SymbioticTS.
 * All changes will be lost the next time the file is generated.
 */

/* tslint:disable:max-line-length */

import { ICircleDto } from './ICircleDto';
import { IRectangleDto } from './IRectangleDto';
import { IShapeDto } from './IShapeDto';
import { IViewModelBaseDto } from './IViewModelBaseDto';

export interface IShapeViewModelDto extends IViewModelBaseDto {
    shapes?: IShapeDto[];
    readonly totalShapes: number;
    readonly rectangles: IRectangleDto[];
    readonly totalRectangles?: number;
    readonly circles: ICircleDto[];
    readonly totalCircles: number;
}
