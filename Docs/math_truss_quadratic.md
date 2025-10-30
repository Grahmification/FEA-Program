## 1D Truss Element (Quadratic)

A 1D quadratic truss element is slightly more complex than the linear case:
- 3 Nodes
  - Only the first 2 nodes (the outermost) are used for returning results. The middle (3rd) node is only used for calculating internal element parameters.
- Each node has 1 DOF ($x_i$)
- $L_e$ is the non-deformed length of the element ($abs(x_2-x_1$))
- $A_e$ cross sectional area of the element

#### Displacement and Shape Function

$$
    q_{3x1}
    =
    \begin{bmatrix}
        q_1 \\
        q_2 \\
        q_3 \\
    \end{bmatrix}
$$

The shape function and local displacement depend on one parameter, since the element has a single dimension (length).

$$
    N(\zeta)_{1x3}
    =
    \begin{bmatrix}
        \frac{-\zeta(1-\zeta)}{2} & \frac{\zeta(1-\zeta)}{2} & (1+\zeta)(1-\zeta)
    \end{bmatrix}
$$

$$
    -1 < \zeta < 1
$$

$$
    u_{1x1} = Nq
$$

#### Stress and Strain

Stress and strain vary linearly throughout the element in a quadratic truss element because the B matrix is a function of $\zeta$.

$$
    B_{1x2}
    = \frac{1}{L_e}
    \begin{bmatrix}
        \frac{-(1-2\zeta)}{2} & \frac{1+2\zeta}{2} & -2\zeta
    \end{bmatrix}
$$

$$
    D_{1x1} = [E]
$$

$$
    \epsilon_{1x1} = Bq
$$

$$
    \sigma_{1x1} = DBq
$$

#### Stiffness Matrix

The stiffness matrix must be integrated over the range of $\zeta$ because of the variable strain matrix.

$$
    K_{3x3} = A_eL_e*D\int_{-1}^{1} B^TB \,d\zeta\ = \frac{EA_e}{3L_e}
    \begin{bmatrix}
    7 & 1 & -8\\
    1 & 7 & -8\\
    -8 & -8 & 16
    \end{bmatrix}
$$


#### Forces

Body Force:

$$
    P_{3x1} = FA_eL_e
    \begin{bmatrix}
        1/6 \\
        1/6 \\
        1/3
    \end{bmatrix}
$$

Traction Force:

$$
    T_{3x1} = TL_e
    \begin{bmatrix}
        1/6 \\
        1/6 \\
        2/3
    \end{bmatrix}
$$
