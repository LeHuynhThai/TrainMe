import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const Login = () => {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [formData, setFormData] = useState({ userName: '', password: '' });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    const result = await login(formData.userName, formData.password);
    if (result.success) {
      navigate('/dashboard');
    } else {
      setError(result.message);
    }
    setLoading(false);
  };

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  return (
    <div className="auth-page">
      {/* Header */}
      <header className="auth-header">
        <div className="auth-header-content">
          <div className="header-logo">
            <div className="logo-icon">TM</div>
            <span className="logo-text">TrainMe</span>
          </div>
          <nav className="nav-menu">
            <span className="nav-item">Dashboard</span>
            <span className="nav-item">Courses</span>
            <span className="nav-item">Progress</span>
            <span className="nav-item">About</span>
          </nav>
        </div>
      </header>

      {/* Main Content */}
      <div className="auth-content">
        <div className="auth-card">
          <div className="auth-card-header">
            <h2>Đăng nhập</h2>
            <p>Chào mừng trở lại TrainMe</p>
          </div>

          {error && (
            <div className="alert alert-danger">
              <strong>Lỗi:</strong> {error}
            </div>
          )}

          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label className="form-label">Tên đăng nhập</label>
              <input
                type="text"
                name="userName"
                className="form-control"
                placeholder="Nhập tên đăng nhập"
                value={formData.userName}
                onChange={handleChange}
                required
                minLength="3"
              />
            </div>

            <div className="form-group">
              <label className="form-label">Mật khẩu</label>
              <input
                type="password"
                name="password"
                className="form-control"
                placeholder="Nhập mật khẩu (3-8 ký tự)"
                value={formData.password}
                onChange={handleChange}
                required
                minLength="3"
                maxLength="8"
              />
            </div>

            <button
              type="submit"
              disabled={loading}
              className={`btn btn-primary ${loading ? 'loading' : ''}`}
            >
              {loading ? 'Đang đăng nhập...' : 'Đăng nhập'}
            </button>
          </form>

          <div className="auth-footer">
            <p className="text-muted">Chưa có tài khoản?</p>
            <Link to="/register" className="btn btn-secondary">
              Đăng ký ngay
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Login;
