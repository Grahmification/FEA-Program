### Description

FEA Program is a piece of software for solving simple finite element analysis (FEA) stress problems. It's created partially to be a useful tool for highly specific stress analysis, and partially as a practical exercise to better understand how FEA theory works.

This software is written in C# and uses WPF for the GUI.

### License

![GitHub](https://img.shields.io/github/license/Grahmification/FEA-Program) FEA Program is available for free under the MIT license.

### Dependencies

FEA Program utilizes the following libraries:
- The [Fody Library](https://github.com/Fody/Fody) under the MIT License. Copyright (c) Simon Cropp.
- The [MathNet.Numerics Library](https://github.com/mathnet/mathnet-numerics) under the MIT License. Copyright (c) 2002-2021 Math.NET.
- The [Helix Toolkit Library](https://github.com/helix-toolkit/helix-toolkit) Library under the MIT License. Copyright (c) 2023 Helix Toolkit contributors.

This software also features Uicons by <a href="https://www.flaticon.com/uicons">Flaticon</a>.

### Getting Started

1. Open `FEA Program.sln` in Visual Studio.
2. Compile the code in Visual Studio.
3. Run the executable file (FEA Program.exe).
4. Example problem files can be found at [Examples](Examples/).

Todo: Make detailed usage instructions.

### Finite Element Theory

An overview of the equations used in this software can be found [here](Docs/math_general.md).


The following external references are useful for learning about FEA theory:
- [Finite Element Trusses](https://www.unm.edu/~bgreen/ME360/Finite%20Element%20Truss.pdf) - Derivation of 2D truss elements, University of New Mexico
- [Introduction to Finite Element Analysis](https://www.engr.uvic.ca/~mech410/lectures/FEA_Theory.pdf) - Derivation of 2D truss elements (linear, quadratic) and beam elements, University of Victoria
- [FEA For Plane Solids](https://web.mae.ufl.edu/nkim/IntroFEA/Chapter6.pdf) - Derivation for 2D triangular and rectangular elements, University of Florida
- [Two Dimensional CST Elements](https://www.meil.pw.edu.pl/content/download/56403/294647/file/2D%20cases%20and%20CST%20triangle%20(lecture_3_part_3).pdf) - Derivation for 2D triangular CST element, Warsaw University of Technology