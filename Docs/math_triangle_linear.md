## 2D Triangle Element (Linear)

The 2D linear triangular element is also known as the CST (constant stress triangle) element because of it's constant stress property. It has the following properties:
- 3 Nodes
- Each node has 2 DOFs ($x_i$, $y_i$)
- $t_e$ is cross sectional thickness of the element

#### Displacement and Shape Function

Displacements are ordered clockwise around the element from node 1 to 3. 

$$
    q_{6x1}
    =
    \begin{bmatrix}
        q_1 \\
        q_2 \\
        \vdots \\
        q_6
    \end{bmatrix}
    = 
        \begin{bmatrix}
        q_{node1x} \\
        q_{node1y} \\
        \vdots \\
        q_{node3y}
    \end{bmatrix}
$$

The shape function and local displacement depends on two parameters, since the element has two dimensions.

$$
    N(\zeta, \eta)_{2x6}
    =
    \begin{bmatrix}
        \zeta & 0 & \eta & 0 & N_3 & 0\\
        0 & \zeta & 0 & \eta & 0 & N_3
    \end{bmatrix}    
$$

$$
N_3 = 1-\zeta-\eta
$$
$$
    u_{2x1} = 
    \begin{bmatrix}
        u\\
        v
    \end{bmatrix}   
    = Nq
$$

#### Geometry

The jacobian faciliates isoparametric mapping to transform the general, often distorted, physical element into simple local coordinates.

$$
    J_{2x2}
    =
    \begin{bmatrix}
        x_{13} & y_{13}\\
        x_{23} & y_{23}
    \end{bmatrix}
$$

where

$$
    x_{ij} = x_i - x_j
$$
    
$$
    y_{ij} = y_i - y_j
$$

The element area can be computed from the jacobian:

$$
    A_e = \frac{|det J|}{2} 
$$

$$
    det J = x_{13}y_{23} - x_{23}y_{13}
$$

#### Strain Matrix

Strain has 3 values, but all are constant throughout the element.

$$
    \epsilon_{3x1} =
    \begin{bmatrix}
        \epsilon_x\\
        \epsilon_y\\
        \gamma_{xy}\\
    \end{bmatrix}   
    = Bq
$$

$$
    B_{3x6}
    = \frac{1}{det J}
    \begin{bmatrix}
        y_{23} & 0 & y_{31} & 0 & y_{12} & 0\\
        0 & x_{32} & 0 & x_{13} & 0 & x_{21}\\
        x_{32} & y_{23} & x_{13} & y_{31} & x_{21} & y_{12}
    \end{bmatrix}
$$



#### Stress Matrix

Stress has 3 values, but all are constant throughout the element.

$$
    \sigma_{3x1} =
    \begin{bmatrix}
        \sigma_x\\
        \sigma_y\\
        \tau_{xy}\\
    \end{bmatrix}   
    = DBq
$$

$$
    D_{3x3}
    = \frac{E}{1-v^2}
    \begin{bmatrix}
        1 & v & 0\\
        v & 1 & 0\\
        0 & 0 & \frac{1-v}{2}
    \end{bmatrix}
$$

#### Stiffness Matrix

$$
    K_{6x6} = A_et_e*B^TDB
$$
