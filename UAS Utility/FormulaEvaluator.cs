using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UAS_Utility
{
    public class FormulaEvaluator
    {
        public double Evaluate(
            string formula,
            IReadOnlyDictionary<string, double> variables)
        {
            // Extract the right-hand side:
            // {LOSResult} = {HeightBuffer} + {HighestPeak}

            var expressionText = formula;

            // Convert {VariableName} into NCalc [VariableName]
            expressionText = Regex.Replace(
                expressionText,
                @"\{([A-Za-z_]\w*)\}",
                "[$1]"
            );

            var expression = new NCalc.Expression(expressionText);

            foreach (var variable in variables)
            {
                expression.Parameters[variable.Key] = variable.Value;
            }

            return Convert.ToDouble(
                expression.Evaluate(),
                CultureInfo.InvariantCulture
            );
        }
    }
}
