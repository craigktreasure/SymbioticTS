﻿/**
 * This file was generated by SymbioticTS.
 * All changes will be lost the next time the file is generated.
 */

/* tslint:disable:max-line-length */

import { Color } from './Color';
import { ShapeBorder } from './ShapeBorder';

export class Triangle {
    public border?: ShapeBorder;
    public readonly color: Color;
    public readonly sides: number;

    constructor(color: Color, sides: number, border?: ShapeBorder) {
        this.color = color;
        this.sides = sides;
        this.border = border;
    }
}
