import React, { useState, useEffect } from 'react';
import { bmiAPI } from '../services/api';
import Header from './Header';

/**
 * BMI Calculator Component
 * Provides functionality to calculate BMI and display health recommendations
 */
const BmiCalculator = () => {
  // Form data state for height and weight inputs
  const [formData, setFormData] = useState({
    height: '',
    weight: ''
  });

  // BMI calculation result state
  const [result, setResult] = useState(null);

  // BMI categories reference data
  const [categories, setCategories] = useState([]);

  // Loading state for API calls
  const [loading, setLoading] = useState(false);

  // Error message state
  const [error, setError] = useState('');

  /**
   * Load BMI categories on component mount
   * Fetches the reference table of BMI categories from the API
   */
  useEffect(() => {
    loadCategories();
  }, []);

  /**
   * Loads BMI categories from the API
   * Used to populate the reference table on the right side
   */
  const loadCategories = async () => {
    try {
      const response = await bmiAPI.getCategories();
      if (response.data.success) {
        setCategories(response.data.data);
      }
    } catch (err) {
      console.error('Error loading BMI categories:', err);
    }
  };

  /**
   * Handles input field changes
   * Updates form data and clears previous results/errors
   * @param {Event} e - Input change event
   */
  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));

    // Clear previous results when input changes
    if (result) {
      setResult(null);
    }
    if (error) {
      setError('');
    }
  };

  /**
   * Calculates BMI using the API
   * Validates input data and sends request to backend
   * @param {Event} e - Form submit event
   */
  const calculateBmi = async (e) => {
    e.preventDefault();

    const height = parseFloat(formData.height);
    const weight = parseFloat(formData.weight);

    // Client-side validation
    if (!height || !weight) {
      setError('Vui lòng nhập đầy đủ chiều cao và cân nặng');
      return;
    }

    if (height < 0.5 || height > 3.0) {
      setError('Chiều cao phải từ 0.5m đến 3.0m');
      return;
    }

    if (weight < 10 || weight > 500) {
      setError('Cân nặng phải từ 10kg đến 500kg');
      return;
    }

    try {
      setLoading(true);
      setError('');

      // Call BMI calculation API
      const response = await bmiAPI.calculateBmi({
        height: height,
        weight: weight
      });

      if (response.data.success) {
        setResult(response.data.data);
      } else {
        setError(response.data.message || 'Có lỗi xảy ra khi tính BMI');
      }
    } catch (err) {
      console.error('Error calculating BMI:', err);
      setError('Có lỗi xảy ra khi tính BMI');
    } finally {
      setLoading(false);
    }
  };

  /**
   * Resets the form to initial state
   * Clears all input fields, results, and error messages
   */
  const resetForm = () => {
    setFormData({ height: '', weight: '' });
    setResult(null);
    setError('');
  };

  /**
   * Returns appropriate color for BMI category
   * Used for visual coding of different BMI ranges
   * @param {string} category - BMI category name
   * @returns {string} - Hex color code
   */
  const getBmiCategoryColor = (category) => {
    switch (category) {
      case 'Thiếu cân nghiêm trọng':
      case 'Thiếu cân':
        return '#3b82f6'; // Blue - Underweight
      case 'Bình thường':
        return '#22c55e'; // Green - Normal
      case 'Thừa cân':
        return '#f59e0b'; // Yellow - Overweight
      case 'Béo phì độ I':
        return '#f97316'; // Orange - Obesity Class I
      case 'Béo phì độ II':
      case 'Béo phì độ III':
        return '#ef4444'; // Red - Obesity Class II & III
      default:
        return '#6b7280'; // Gray - Default
    }
  };

  return (
    <div className="bmi-page">
      {/* Header navigation */}
      <Header />

      <div className="bmi-container">
        {/* Page title and description */}
        <div className="bmi-header">
          <h1>Máy Tính BMI</h1>
          <p>Tính chỉ số khối cơ thể (BMI) và nhận lời khuyên sức khỏe</p>
        </div>

        {/* Main content with two equal-height columns */}
        <div className="bmi-content">
          {/* Left column: BMI Calculator Form */}
          <div className="bmi-calculator">
            <h2>Tính BMI của bạn</h2>
            
            <form onSubmit={calculateBmi} className="bmi-form">
              <div className="form-group">
                <label htmlFor="height">Chiều cao (m)</label>
                <input
                  type="number"
                  id="height"
                  name="height"
                  value={formData.height}
                  onChange={handleInputChange}
                  onInvalid={(e) => e.target.setCustomValidity('Chiều cao phải lớn hơn hoặc bằng 0.5 m và nhỏ hơn hoặc bằng 3.0 m')}
                  onInput={(e) => e.target.setCustomValidity('')}
                  placeholder="Ví dụ: 1.70"
                  step="0.01"
                  min="0.5"
                  max="3.0"
                  className="form-input"
                />
              </div>

              <div className="form-group">
                <label htmlFor="weight">Cân nặng (kg)</label>
                <input
                  type="number"
                  id="weight"
                  name="weight"
                  value={formData.weight}
                  onChange={handleInputChange}
                  onInvalid={(e) => e.target.setCustomValidity('Cân nặng phải từ 10 kg đến 500 kg')}
                  onInput={(e) => e.target.setCustomValidity('')}
                  placeholder="Ví dụ: 70"
                  step="0.1"
                  min="10"
                  max="500"
                  className="form-input"
                />
              </div>

              {error && (
                <div className="error-message">
                  {error}
                </div>
              )}

              <div className="form-actions">
                <button
                  type="submit"
                  className="btn btn-primary calculate-btn"
                  disabled={loading}
                >
                  {loading ? 'Đang tính...' : 'Tính BMI'}
                </button>
                
                <button
                  type="button"
                  onClick={resetForm}
                  className="btn btn-secondary reset-btn"
                >
                  Làm mới
                </button>
              </div>
            </form>

            {/* BMI Result */}
            {result && (
              <div className="bmi-result">
                <h3>Kết quả BMI</h3>
                <div className="result-card">
                  <div className="bmi-value">
                    <span className="bmi-number">{result.bmiValue}</span>
                    <span className="bmi-unit">BMI</span>
                  </div>
                  
                  <div 
                    className="bmi-category"
                    style={{ color: getBmiCategoryColor(result.category) }}
                  >
                    {result.category}
                  </div>
                  
                  <div className="bmi-description">
                    {result.description}
                  </div>
                  
                  <div className="health-advice">
                    <h4>Lời khuyên sức khỏe:</h4>
                    <p>{result.healthAdvice}</p>
                  </div>
                  
                  <div className="calculation-info">
                    <small>
                      Chiều cao: {result.height}m | Cân nặng: {result.weight}kg
                    </small>
                  </div>
                </div>
              </div>
            )}
          </div>

          {/* Right column: BMI Categories Reference Table */}
          <div className="bmi-categories">
            <h2>Bảng phân loại BMI</h2>
            <div className="categories-list">
              {/* Render each BMI category with color coding */}
              {categories.map((category, index) => (
                <div key={index} className="category-item">
                  {/* Color indicator for visual reference */}
                  <div
                    className="category-indicator"
                    style={{ backgroundColor: getBmiCategoryColor(category.category) }}
                  ></div>
                  {/* Category information */}
                  <div className="category-info">
                    <div className="category-name">{category.category}</div>
                    <div className="category-range">
                      BMI: {category.minBmi} - {category.maxBmi || '∞'}
                    </div>
                    <div className="category-description">{category.description}</div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default BmiCalculator;
