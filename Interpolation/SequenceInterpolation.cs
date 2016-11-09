using System.Collections.Generic;

namespace InterpolationLib
{
    public static class SequenceInterpolation
    {
        public static float[] Lerp(float zeroValue, float unitValue, int stepsCount)
        {
            float[] values = new float[stepsCount];
            float stepSize = (float)1 / stepsCount;
            for (int i = 0; i < stepsCount; i++)
                values[i] = Interpolation.Lerp(zeroValue, unitValue, stepSize * i);
            return values;
        }

        public static float[] Derp(float zeroValue, float unitValue, float zeroDerivative, float unitDerivative,
            int stepsCount)
        {
            float[] values = new float[stepsCount];
            float stepSize = (float)1 / stepsCount;
            for (int i = 0; i < stepsCount; i++)
                values[i] = Interpolation.Derp(zeroValue, unitValue, zeroDerivative, unitDerivative, stepSize * i);
            return values;
        }

        public static float[] LerpByNodes(float[] nodes, int steps)
        {
            List<float> result = new List<float>();
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                float zeroNode = nodes[i];
                float unitNode = nodes[i + 1];
                float[] values = Lerp(zeroNode, unitNode, steps);
                result.AddRange(values);
            }
            return result.ToArray();
        }

        public static float[] DerpByNodes(float[] valuesNodes, float[] derivativesNodes, int steps)
        {
            List<float> result = new List<float>();
            for (int i = 0; i < valuesNodes.Length - 1; i++)
            {
                float zeroValue = valuesNodes[i];
                float unitValue = valuesNodes[i + 1];
                float zeroDerivative = derivativesNodes[i];
                float unitDerivative = derivativesNodes[i + 1];
                float[] values = Derp(zeroValue, unitValue, zeroDerivative, unitDerivative, steps);
                result.AddRange(values);
            }
            return result.ToArray();
        }

        public static float[] MultiDerpSequenceByNodes(List<float[]> nodesLayers, int steps)
        {
            nodesLayers.Reverse();
            float[] derivativesLayer = LerpByNodes(nodesLayers[0], steps);
            for (int i = 1; i < nodesLayers.Count; i++)
            {
                var valuesLayer = nodesLayers[i];
                derivativesLayer = DerpByNodes(valuesLayer, derivativesLayer, steps);
            }
            return derivativesLayer;
        }
    }
}
