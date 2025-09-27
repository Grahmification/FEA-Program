using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FEA_Program.Models.MaterialMgr;

namespace FEA_Program.Models
{
    internal class MaterialClass
    {
        private string _Name = "";
        private double _E = 0d; // youngs modulus in Pa
        private double _V = 0d; // poissons ratio
        private double _Sy = 0d; // yield strength in Pa
        private double _Sut = 0d; // ultimate strength in Pa
        private MaterialType _subtype;
        private int _ID = -1;

        public int ID
        {
            get
            {
                return _ID;
            }
        }
        public string Name
        {
            get
            {
                return _Name;
            }
        }
        public double Sy_MPa
        {
            get
            {
                return _Sy / (1000.0d * 1000.0d);
            }
        }
        public double Sut_MPa
        {
            get
            {
                return _Sut / (1000.0d * 1000.0d);
            }
        }
        public double E_GPa
        {
            get
            {
                return _E / (1000 * 1000 * 1000); // convert to GPa
            }
        }
        public double E
        {
            get
            {
                return _E;
            }
        }
        public double V
        {
            get
            {
                return _V;
            }
        }
        public MaterialType Subtype
        {
            get
            {
                return _subtype;
            }
        }
        public DenseMatrix D_matrix_2D
        {
            get
            {
                var output = new DenseMatrix(3, 3);
                output.Clear(); // set all vals to 0

                output[0, 0] = 1;
                output[0, 1] = _V;
                output[1, 0] = _V;
                output[1, 1] = 1;
                output[2, 2] = (1d - _V) / 2d;

                return (DenseMatrix)(output * _E / (1d - _V * _V));
            }
        }

        public MaterialClass(string Name, double E_GPa, double V, double Sy_MPa, double Sut_MPa, MaterialType subtype, int InputID)
        {

            _Name = Name;
            _E = E_GPa * 1000d * 1000d * 1000d; // convert to Pa
            _V = V;
            _Sy = Sy_MPa * 1000d * 1000d; // convert to Pa
            _Sut = Sut_MPa * 1000d * 1000d; // convert to Pa
            _subtype = subtype;
            _ID = InputID;
        }
    }

    public enum MaterialType
    {
        Steel_Alloy,
        Aluminum_Alloy
    }
}
