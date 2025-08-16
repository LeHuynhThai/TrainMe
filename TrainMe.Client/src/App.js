import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import Login from './components/Login';
import Register from './components/Register';
import WorkoutPage from './components/WorkoutPage';
import BmiCalculator from './components/BmiCalculator';

// Loading Screen Component
const LoadingScreen = () => (
  <div className="loading-screen">
    <div className="loading-card">
      <div className="loading-spinner">🚀</div>
      <h2>TrainMe</h2>
      <p>Đang tải...</p>
    </div>
  </div>
);

// Protected Route - Chỉ cho phép user đã login
const PrivateRoute = ({ children }) => {
  const { user, loading } = useAuth();
  
  if (loading) return <LoadingScreen />;
  return user ? children : <Navigate to="/login" />;
};

// Public Route - Chỉ cho phép user chưa login
const PublicRoute = ({ children }) => {
  const { user, loading } = useAuth();

  if (loading) return <LoadingScreen />;
  return user ? <Navigate to="/workout" /> : children;
};

function App() {
  return (
    <AuthProvider>
      <Router>
        <div className="App">
          <Routes>
            {/* Public routes */}
            <Route path="/login" element={
              <PublicRoute>
                <Login />
              </PublicRoute>
            } />
            
            <Route path="/register" element={
              <PublicRoute>
                <Register />
              </PublicRoute>
            } />
            
            {/* Protected routes */}
            <Route path="/workout" element={
              <PrivateRoute>
                <WorkoutPage />
              </PrivateRoute>
            } />

            <Route path="/bmi" element={
              <PrivateRoute>
                <BmiCalculator />
              </PrivateRoute>
            } />

            {/* Default redirect */}
            <Route path="/" element={<Navigate to="/workout" />} />
          </Routes>
        </div>
      </Router>
    </AuthProvider>
  );
}

export default App;
