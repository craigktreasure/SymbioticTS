﻿/**
 * This file was generated by SymbioticTS.
 * All changes will be lost the next time the file is generated.
 */

/* tslint:disable:max-line-length */

import { BaseShape } from './BaseShape';
import { Color } from './Color';
import { IQuadrilateral } from './IQuadrilateral';
import { IRectangleDto } from './IRectangleDto';
import { IShape } from './IShape';
import { ShapeBorder } from './ShapeBorder';

export class Rectangle extends BaseShape implements IShape, IQuadrilateral {
    public readonly height: number;
    public readonly width: number;

    constructor(
        color: Color,
        height: number,
        sides: number,
        width: number,
        border?: ShapeBorder) {
        super(color, sides, border);

        this.height = height;
        this.width = width;
    }

    public static fromDto(dto: IRectangleDto): Rectangle {
        return new Rectangle(
            dto.color,
            dto.height,
            dto.sides,
            dto.width,
            dto.border);
    }
}
