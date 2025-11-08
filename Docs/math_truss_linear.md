## 1D Truss Element (Linear)

The following equations are for the simplest element, a 1 dimensional truss. It has the following properties:
- 2 Nodes
- Each node has 1 DOF ($x_i$)
- $L_e$ is the non-deformed length of the element ($abs(x_2-x_1$))
- $A_e$ cross sectional area of the element

#### Displacement and Shape Function

$$
    q_{2x1}
    =
    \begin{bmatrix}
        q_1 \\
        q_2 \\
    \end{bmatrix}
$$

The shape function and local displacement depend on one parameter, since the element has a single dimension (length).

$$
    N(\zeta)_{1x2}
    =
    \begin{bmatrix}
        \frac{1-\zeta}{2} & \frac{1+\zeta}{2}
    \end{bmatrix}
$$

$$
    -1 < \zeta < 1
$$

$$
    u_{1x1} = Nq
$$

#### Stress and Strain

Both stress and strain have constant values throughout the element.

$$
    B_{1x2}
    = \frac{1}{L_e}
    \begin{bmatrix}
        -1 & 1
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

$$
    K_{2x2} = A_eL_e*B^TDB = \frac{EA_e}{L_e}
    \begin{bmatrix}
    1 & -1\\
    -1 & 1
    \end{bmatrix}
$$


#### Forces

Body Force:

$$
    P_{2x1} = F\frac{A_eL_e}{2}
    \begin{bmatrix}
        1 \\
        1
    \end{bmatrix}
$$

Traction Force:

$$
    T_{2x1} = T\frac{L_e}{2}
    \begin{bmatrix}
        1 \\
        1
    \end{bmatrix}
$$

## 2D Truss Element (Linear)

A 2D truss element is similar to the 1D case:
- 2 Nodes
- Each node has 2 DOFs ($x_i$, $y_i$)
- $L_e$ is the non-deformed length of the element (Vector length)
- $A_e$ cross sectional area of the element

#### Transform

A special transform is used for 2D truss elements to convert coordinates to the 1D case.

$$
    q_{2x1}' = Mq
$$

with the following matricies

$$
    q_{2x1}'
    =
    \begin{bmatrix}
        q_1'\\
        q_2'\\
    \end{bmatrix}
$$

$$
    M_{2x4}
    =
    \begin{bmatrix}
        l & m & 0 & 0\\
        0 & 0 & l & m\\
    \end{bmatrix}
$$

$$
    q_{4x1}
    =
    \begin{bmatrix}
        q_1\\
        q_2\\
        q_3\\
        q_4\\
    \end{bmatrix}
$$

where
- $l$ is the X component of the element's unit normalized length vector
- $m$ is the Y component of the element's unit normalized length vector

Once the local coordinates have been computed, many of the same equations can be used as for the 1D case:

$$
    u_{1x1} = Nq' = NMq
$$

$$
    \epsilon_{1x1} = Bq' = BMq
$$

$$
    \sigma_{1x1} = DBq' = DBMq
$$

#### Stiffness Matrix

The stiffness matrix calculation changes slightly:

$$
    K_{4x4} = M^TK_{1d}M = \frac{EA_e}{L_e}
    \begin{bmatrix}
        l^2 & lm & -l^2 & -lm\\
        lm & m^2 & -lm & -m^2\\
        -l^2 & -lm & l^2 & lm\\
        -lm & -m^2 & lm & m^2\\
    \end{bmatrix}
$$

#### Forces

The body and traction force vectors also change slightly:

$$
    P_{4x1} = \frac{A_eL_e}{2}
    \begin{bmatrix}
        Fx\\
        Fy
    \end{bmatrix}
    \begin{bmatrix}
        1 & 0\\
        0 & 1\\
    \end{bmatrix}
$$

$$
    T_{4x1} = \frac{L_e}{2}
    \begin{bmatrix}
        Tx\\
        Ty
    \end{bmatrix}
    \begin{bmatrix}
        1 & 0\\
        0 & 1\\
    \end{bmatrix}
$$


## 3D Truss Element (Linear)

A 3D truss element is similar to the 1D and 2D cases:
- 2 Nodes
- Each node has 3 DOFs ($x_i$, $y_i$, $z_i$)
- $L_e$ is the non-deformed length of the element (Vector length)
- $A_e$ cross sectional area of the element

#### Transform

Like the 2D truss, a special transform is used for 3D truss elements to convert coordinates to the 1D case.

$$
    q_{2x1}' = Mq
$$

with the following matricies

$$
    q_{2x1}'
    =
    \begin{bmatrix}
        q_1'\\
        q_2'\\
    \end{bmatrix}
$$

$$
    M_{2x6}
    =
    \begin{bmatrix}
        l & m & n & 0 & 0 & 0\\
        0 & 0 & 0 & l & m & n
    \end{bmatrix}
$$

$$
    q_{6x1}
    =
    \begin{bmatrix}
        q_1\\
        q_2\\
        q_3\\
        q_4\\
        q_5\\
        q_6\\
    \end{bmatrix}
$$

where
- $l$ is the X component of the element's unit normalized length vector
- $m$ is the Y component of the element's unit normalized length vector
- $n$ is the Z component of the element's unit normalized length vector

Like the 2D element, once the local coordinates have been computed, many of the same equations can be used as for the 1D case.

#### Stiffness Matrix

The stiffness matrix calculation changes slightly:

$$
    K_{6x6} = M^TK_{1d}M
$$

#### Forces

The body and traction force vectors also change slightly:

$$
    P_{6x1} = \frac{A_eL_e}{2}
    \begin{bmatrix}
        Fx\\
        Fy\\
        Fz
    \end{bmatrix}
    \begin{bmatrix}
        1 & 0 & 0\\
        0 & 1 & 0\\
        0 & 0 & 1\\
        1 & 0 & 0\\
        0 & 1 & 0\\
        0 & 0 & 1\\
    \end{bmatrix}
$$

$$
    T_{6x1} = \frac{L_e}{2}
    \begin{bmatrix}
        Tx\\
        Ty\\
        Tx
    \end{bmatrix}
    \begin{bmatrix}
        1 & 0 & 0\\
        0 & 1 & 0\\
        0 & 0 & 1\\
        1 & 0 & 0\\
        0 & 1 & 0\\
        0 & 0 & 1\\
    \end{bmatrix}
$$