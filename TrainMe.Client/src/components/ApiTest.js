import React, { useState } from 'react';
import { authAPI } from '../services/api';

const ApiTest = () => {
  const [result, setResult] = useState('');
  const [loading, setLoading] = useState(false);

  const testRegister = async () => {
    setLoading(true);
    setResult('Testing register...');

    try {
      const response = await authAPI.register({
        userName: 'testuser',
        password: 'Test123!'
      });

      console.log('Register response:', response);
      setResult(JSON.stringify(response.data, null, 2));
    } catch (error) {
      console.error('Register error:', error);
      setResult(`Error: ${error.message}\nResponse: ${JSON.stringify(error.response?.data, null, 2)}`);
    } finally {
      setLoading(false);
    }
  };

  const testLogin = async () => {
    setLoading(true);
    setResult('Testing login...');
    
    try {
      const response = await authAPI.login({
        userName: 'testuser',
        password: 'Test123!'
      });
      
      console.log('Login response:', response);
      setResult(JSON.stringify(response.data, null, 2));
    } catch (error) {
      console.error('Login error:', error);
      setResult(`Error: ${error.message}\nResponse: ${JSON.stringify(error.response?.data, null, 2)}`);
    } finally {
      setLoading(false);
    }
  };

  const testFullFlow = async () => {
    setLoading(true);
    setResult('Testing full authentication flow...\n\n');

    try {
      // Step 1: Register
      setResult(prev => prev + '1. Testing Register...\n');
      const registerResponse = await authAPI.register({
        userName: 'flowtest' + Date.now(),
        password: 'Test123!'
      });

      if (registerResponse.data.success) {
        setResult(prev => prev + '✅ Register successful\n');
        setResult(prev => prev + `User created: ${registerResponse.data.data.userName}\n\n`);

        // Step 2: Login
        setResult(prev => prev + '2. Testing Login...\n');
        const loginResponse = await authAPI.login({
          userName: registerResponse.data.data.userName,
          password: 'Test123!'
        });

        if (loginResponse.data.success) {
          setResult(prev => prev + '✅ Login successful\n');
          setResult(prev => prev + `Token received: ${loginResponse.data.data.accessToken.substring(0, 50)}...\n\n`);

          // Step 3: Get current user
          setResult(prev => prev + '3. Testing Get Current User...\n');
          localStorage.setItem('token', loginResponse.data.data.accessToken);

          const userResponse = await authAPI.getCurrentUser();
          if (userResponse.data.success) {
            setResult(prev => prev + '✅ Get current user successful\n');
            setResult(prev => prev + `User info: ${JSON.stringify(userResponse.data.data, null, 2)}\n\n`);
            setResult(prev => prev + '🎉 All tests passed! Backend and Frontend are connected properly.');
          } else {
            setResult(prev => prev + '❌ Get current user failed\n');
            setResult(prev => prev + JSON.stringify(userResponse.data, null, 2));
          }
        } else {
          setResult(prev => prev + '❌ Login failed\n');
          setResult(prev => prev + JSON.stringify(loginResponse.data, null, 2));
        }
      } else {
        setResult(prev => prev + '❌ Register failed\n');
        setResult(prev => prev + JSON.stringify(registerResponse.data, null, 2));
      }
    } catch (error) {
      console.error('Full flow error:', error);
      setResult(prev => prev + `❌ Error: ${error.message}\n`);
      if (error.response) {
        setResult(prev => prev + `Response: ${JSON.stringify(error.response.data, null, 2)}`);
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ padding: '20px', maxWidth: '800px', margin: '0 auto' }}>
      <h2>API Connection Test</h2>
      
      <div style={{ marginBottom: '20px' }}>
        <button onClick={testFullFlow} disabled={loading} style={{ marginRight: '10px', padding: '12px 24px', fontSize: '16px', fontWeight: 'bold', background: '#007bff', color: 'white', border: 'none', borderRadius: '8px' }}>
          🚀 Test Full Flow (Register → Login → Get User)
        </button>
        <button onClick={testRegister} disabled={loading} style={{ marginRight: '10px' }}>
          Test Register Only
        </button>
        <button onClick={testLogin} disabled={loading}>
          Test Login Only
        </button>
      </div>
      
      <div style={{ 
        background: '#f5f5f5', 
        padding: '20px', 
        borderRadius: '8px',
        minHeight: '200px',
        fontFamily: 'monospace',
        whiteSpace: 'pre-wrap'
      }}>
        {loading ? 'Loading...' : result || 'Click a button to test API connection'}
      </div>
    </div>
  );
};

export default ApiTest;
