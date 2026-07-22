using UnityEngine;

namespace Utils
{
    public static class Maths
    {
        /// <summary>
        /// Small value used for float comparisons to account for floating-point precision errors
        /// </summary>
        public const float Epsilon = 1e-6f;
        
        /// <summary>
        /// Compares two float values for approximate equality using epsilon tolerance
        /// </summary>
        /// <param name="a">First float value</param>
        /// <param name="b">Second float value</param>
        /// <returns>True if the values are approximately equal within epsilon tolerance</returns>
        public static bool ApproximatelyEqual(float a, float b)
        {
            return Mathf.Abs(a - b) < Epsilon;
        }
        
        /// <summary>
        /// Compares two float values for approximate equality using custom epsilon tolerance
        /// </summary>
        /// <param name="a">First float value</param>
        /// <param name="b">Second float value</param>
        /// <param name="epsilon">Custom epsilon tolerance value</param>
        /// <returns>True if the values are approximately equal within the specified tolerance</returns>
        public static bool ApproximatelyEqual(float a, float b, float epsilon)
        {
            return Mathf.Abs(a - b) < epsilon;
        }
        
        /// <summary>
        /// Checks if a float value is approximately zero using epsilon tolerance
        /// </summary>
        /// <param name="value">Float value to check</param>
        /// <returns>True if the value is approximately zero within epsilon tolerance</returns>
        public static bool ApproximatelyZero(float value)
        {
            return Mathf.Abs(value) < Epsilon;
        }
        
        /// <summary>
        /// Checks if a float value is approximately zero using custom epsilon tolerance
        /// </summary>
        /// <param name="value">Float value to check</param>
        /// <param name="epsilon">Custom epsilon tolerance value</param>
        /// <returns>True if the value is approximately zero within the specified tolerance</returns>
        public static bool ApproximatelyZero(float value, float epsilon)
        {
            return Mathf.Abs(value) < epsilon;
        }
        
        /// <summary>
        /// Checks if the first float value is approximately less than the second using epsilon tolerance
        /// </summary>
        /// <param name="a">First float value</param>
        /// <param name="b">Second float value</param>
        /// <returns>True if a is less than b beyond epsilon tolerance</returns>
        public static bool ApproximatelyLessThan(float a, float b)
        {
            return a < b - Epsilon;
        }
        
        /// <summary>
        /// Checks if the first float value is approximately less than the second using custom epsilon tolerance
        /// </summary>
        /// <param name="a">First float value</param>
        /// <param name="b">Second float value</param>
        /// <param name="epsilon">Custom epsilon tolerance value</param>
        /// <returns>True if a is less than b beyond the specified tolerance</returns>
        public static bool ApproximatelyLessThan(float a, float b, float epsilon)
        {
            return a < b - epsilon;
        }
        
        /// <summary>
        /// Checks if the first float value is approximately greater than the second using epsilon tolerance
        /// </summary>
        /// <param name="a">First float value</param>
        /// <param name="b">Second float value</param>
        /// <returns>True if a is greater than b beyond epsilon tolerance</returns>
        public static bool ApproximatelyGreaterThan(float a, float b)
        {
            return a > b + Epsilon;
        }
        
        /// <summary>
        /// Checks if the first float value is approximately greater than the second using custom epsilon tolerance
        /// </summary>
        /// <param name="a">First float value</param>
        /// <param name="b">Second float value</param>
        /// <param name="epsilon">Custom epsilon tolerance value</param>
        /// <returns>True if a is greater than b beyond the specified tolerance</returns>
        public static bool ApproximatelyGreaterThan(float a, float b, float epsilon)
        {
            return a > b + epsilon;
        }
    }
}
