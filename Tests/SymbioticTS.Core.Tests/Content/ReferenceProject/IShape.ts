﻿/**
 * This file was generated by SymbioticTS.
 * All changes will be lost the next time the file is generated.
 */

/* tslint:disable:max-line-length */

import { Color } from './Color';
import { ShapeBorder } from './ShapeBorder';

export interface IShape {
    readonly border?: ShapeBorder;
    readonly color: Color;
    readonly sides: number;
}
