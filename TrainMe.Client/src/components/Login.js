import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const Login = () => {
  const navigate = useNavigate();
  const { login } = useAuth();

  const [formData, setFormData] = useState({
    userName: '',
    password: ''
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const result = await login(formData.userName, formData.password);
      if (result.success) {
        navigate('/dashboard');
      } else {
        setError(result.message || 'Đăng nhập thất bại');
      }
    } catch (err) {
      setError('Có lỗi xảy ra khi đăng nhập');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container" style={{
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      minHeight: '100vh',
      padding: '20px'
    }}>
      <div className="card" style={{ width: '100%', maxWidth: '400px' }}>
        <div className="card-header">
          <h2 style={{ margin: 0, fontSize: '28px', fontWeight: '700' }}>
            🚀 TrainMe
          </h2>
          <p style={{ margin: '8px 0 0 0', opacity: 0.9 }}>
            Đăng nhập vào tài khoản của bạn
          </p>
        </div>

        <div className="card-body">
          {error && (
            <div className="alert alert-danger">
              <strong>Lỗi!</strong> {error}
            </div>
          )}

          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label className="form-label">
                👤 Tên đăng nhập
              </label>
              <input
                type="text"
                name="userName"
                value={formData.userName}
                onChange={handleChange}
                required
                className="form-control"
                placeholder="Nhập tên đăng nhập của bạn"
                autoComplete="username"
              />
            </div>

            <div className="form-group">
              <label className="form-label">
                🔒 Mật khẩu
              </label>
              <input
                type="password"
                name="password"
                value={formData.password}
                onChange={handleChange}
                required
                className="form-control"
                placeholder="Nhập mật khẩu của bạn"
                autoComplete="current-password"
              />
            </div>

            <button
              type="submit"
              disabled={loading}
              className={`btn btn-primary ${loading ? 'loading' : ''}`}
              style={{ width: '100%', fontSize: '16px', padding: '14px' }}
            >
              {loading ? (
                <>
                  <span style={{ marginRight: '8px' }}>⏳</span>
                  Đang đăng nhập...
                </>
              ) : (
                <>
                  <span style={{ marginRight: '8px' }}>🚪</span>
                  Đăng nhập
                </>
              )}
            </button>
          </form>

          <div className="text-center mt-3">
            <p style={{ color: '#6c757d', marginBottom: '16px' }}>
              Chưa có tài khoản?
            </p>
            <Link
              to="/register"
              className="btn btn-secondary"
              style={{ textDecoration: 'none' }}
            >
              <span style={{ marginRight: '8px' }}>📝</span>
              Đăng ký ngay
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Login;
