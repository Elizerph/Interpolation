namespace InterpolationLib
{
    public static class Interpolation
    {
        public static float Lerp(float zeroValue, float unitValue, float parameter, bool clamp = false)
        {
            if (clamp)
            {
                if (parameter > 1)
                    parameter = 1;
                else
                    if (parameter < 0)
                        parameter = 0;
            }
            float factor = unitValue - zeroValue;
            return zeroValue + parameter * factor;
        }

        public static float Derp(float zeroValue, float unitValue, float zeroDerivative, float unitDerivative, float parameter, bool clamp = false)
        {
            if (clamp)
            {
                if (parameter > 1)
                    parameter = 1;
                else
                    if (parameter < 0)
                        parameter = 0;
            }
            float c1 = zeroValue;
            float c2 = zeroDerivative;
            float c3 = 3*(unitValue - zeroValue) - 2*zeroDerivative - unitDerivative;
            float c4 = unitDerivative - 2*(unitValue - zeroValue) + zeroDerivative;
            float p2 = parameter*parameter;
            float p3 = p2*parameter;
            return c1 + c2*parameter + c3*p2 + c4*p3;
        }
    }
}
