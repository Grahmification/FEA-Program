## General FEA Equations ##

The main FEA equation is given by:

$$
    K_gq_g = F_g + T_g + P_g
$$

Where:
- $K_g$ is the global stiffness matrix [DOFs x DOFs]
- $q_g$ is the global displacement matrix [DOFs x 1]
- $F_g$ is the global force matrix [DOFs x 1]
- $T_g$ is the global traction force matrix [DOFs x 1]
- $P_g$ is the global body force matrix [DOFs x 1]
- DOFs = Problem Size = Number of Nodes in Problem * DOFs in Each Node

In this equation displacement is generally the unknown to be solved for, except in some cases where a displacement is fixed and the force (reaction force) is solved for instead.

 ### General Element Equations

The following section describes generalized equations that apply to all mechanical FEA elements. For information on specific elements, refer to the following:
- (1D) Truss: [Linear](math_truss_linear.md), [Quadratic](math_truss_quadratic.md)
- (2D) Triangle: [Linear](math_triangle_linear.md)


There are two general parameters that determine the size of matricies for an element:
- Node DOFs = Number of Nodes in Element * DOFs in Each Node
- Element DOFs = Number of local directions inside element (1, 2, 3)

#### Shape Function

$$
    u = Nq
$$

Where:
- $u$ is the local displacement matrix inside the element [Element DOFs x 1]
- $N$ is the element shape factor matrix [Element DOFs x Node DOFs]
- $q$ is the node displacement matrix [Node DOFs x 1]

#### Strain Calculation

Note: size of the stress and strain matricies varies depending on the dimension of the element:
- 1 Dimension: [1 x 1]
- 2 Dimension: [3 x 1]
- 3 Dimension: [6 x 1]

The B matrix size varies accordingly, depending on the number of nodes and size of the output matrix.

$$
    \epsilon = Bq
$$

Where:
- $\epsilon$ is the element strain matrix [1 x 1], [3 x 1], [6 x 1]
- $B$ is the strain/displacement matrix [1 x Node DOFs], [3 x Node DOFs], [6 x Node DOFs]
- $q$ is the node displacement matrix [Node DOFs x 1]

#### Stress Calculation

$$
    \sigma = D\epsilon = DBq
$$

Where:
- $\sigma$ is the element stress matrix [1 x 1], [3 x 1], [6 x 1]
- $D$ is the material constitutive matrix [1 x 1], [3 x 3], [6 x 6]
- $q$ is the node displacement matrix [Node DOFs x 1]

#### Stiffness Calculation

Element stiffness is determined integrating the $B$ and $D$ matricies over the element's volume. In many cases these matricies do not depend on position inside the element, and the integral simply becomes the element's volume.

$$
    K = \int_{V}^{} B^TDB \,dV\
$$

Where:
- $K$ is the element stiffness matrix [Node DOFs x Node DOFs]
- $V$ is the element's volume
