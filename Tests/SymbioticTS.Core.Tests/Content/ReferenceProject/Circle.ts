﻿/**
 * This file was generated by SymbioticTS.
 * All changes will be lost the next time the file is generated.
 */

import { BaseShape } from './BaseShape';
import { Color } from './Color';
import { IShape } from './IShape';
import { ShapeBorder } from './ShapeBorder';

export class Circle extends BaseShape implements IShape
{
    public readonly diameter: number;

    constructor(
        color: Color,
        diameter: number,
        sides: number,
        border?: ShapeBorder)
    {
        super(color, sides, border);

        this.diameter = diameter;
    }
}
