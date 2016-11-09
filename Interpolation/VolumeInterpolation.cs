using System;
using System.Collections.Generic;
using System.Linq;

namespace InterpolationLib
{
    public static class VolumeInterpolation
    {
        public static float[,,] LerpByNodes(float[,,] nodes, int steps)
        {
            return nodes.GoThrough3D(rn => SequenceInterpolation.LerpByNodes(rn, steps));
        }

        public static float[,,] DerpByNodes(float[,,] valuesNodes, float[,,] derivativesNodes, int steps)
        {
            int width = valuesNodes.GetLength(0);
            int height = valuesNodes.GetLength(1);
            int depth = valuesNodes.GetLength(2);

            Tuple<float, float>[,,] nodes = new Tuple<float, float>[width, height, depth];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    for (int k = 0; k < depth; k++)
                        nodes[i, j, k] = new Tuple<float, float>(valuesNodes[i, j, k], derivativesNodes[i, j, k]);

            Func<Tuple<float, float>[], Tuple<float, float>[]> selector = t =>
            {
                float[] values = t.Select(e => e.Item1).ToArray();
                float[] derivatives = t.Select(e => e.Item2).ToArray();
                return SequenceInterpolation.DerpByNodes(values, derivatives, steps).Select(e => new Tuple<float, float>(e, 0)).ToArray();
            };

            Tuple<float, float>[,,] resultTuples = nodes.GoThrough3D(selector);

            int resultWidth = resultTuples.GetLength(0);
            int resultHeight = resultTuples.GetLength(1);
            int resultDepth = resultTuples.GetLength(2);
            float[,,] result = new float[resultWidth, resultHeight, resultDepth];
            for (int i = 0; i < resultWidth; i++)
                for (int j = 0; j < resultHeight; j++)
                    for (int k = 0; k < resultDepth; k++)
                        result[i, j, k] = resultTuples[i, j, k].Item1;
            return result;
        }

        
        public static float[,,] MultiDerpByNodes(List<float[,,]> nodesLayers, int steps)
        {
            nodesLayers.Reverse();
            float[,,] derivativesLayer = LerpByNodes(nodesLayers[0], steps);
            for (int i = 1; i < nodesLayers.Count; i++)
            {
                float[,,] valuesLayer = nodesLayers[i];
                derivativesLayer = DerpByNodes(valuesLayer, derivativesLayer, steps);
            }
            return derivativesLayer;
        }
    }
}
