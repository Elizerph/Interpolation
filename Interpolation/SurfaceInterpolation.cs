using System;
using System.Collections.Generic;
using System.Linq;

namespace InterpolationLib
{
    public static class SurfaceInterpolation
    {
        public static float[,] LerpByNodes(float[,] nodes, int steps)
        {
            return nodes.GoThrough2D(cn => SequenceInterpolation.LerpByNodes(cn, steps));
        }

        public static float[,] DerpByNodes(float[,] valuesNodes, float[,] derivativesNodes, int steps)
        {
            int width = valuesNodes.GetLength(0);
            int height = valuesNodes.GetLength(1);
            Tuple<float, float>[,] nodes = new Tuple<float, float>[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    nodes[i, j] = new Tuple<float, float>(valuesNodes[i, j], derivativesNodes[i, j]);

            Func<Tuple<float, float>[], Tuple<float, float>[]> selector = t => 
            {
                float[] values = t.Select(e => e.Item1).ToArray();
                float[] derivatives = t.Select(e => e.Item2).ToArray();
                return SequenceInterpolation.DerpByNodes(values, derivatives, steps).Select(e => new Tuple<float, float>(e, 0)).ToArray();
            };
            
            Tuple<float, float>[,] resultTuples = nodes.GoThrough2D(selector);

            int resultWidth = resultTuples.GetLength(0);
            int resultHeight = resultTuples.GetLength(1);
            float[,] result = new float[resultWidth, resultHeight];
            for (int i = 0; i < resultWidth; i++)
                for (int j = 0; j < resultHeight; j++)
                    result[i, j] = resultTuples[i, j].Item1;
            return result;
        }

        public static float[,] MultiDerpByNodes(List<float[,]> nodesLayers, int steps)
        {
            nodesLayers.Reverse();
            float[,] derivativesLayer = LerpByNodes(nodesLayers[0], steps);
            for (int i = 1; i < nodesLayers.Count; i++)
            {
                float[,] valuesLayer = nodesLayers[i];
                derivativesLayer = DerpByNodes(valuesLayer, derivativesLayer, steps);
            }
            return derivativesLayer;
        }
    }
}
