import React, { useState, useContext } from 'react';
import { Container, Box, TextField, Button, Link, Avatar, Tabs, Tab, Select, MenuItem } from '@mui/material';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import { Navigate } from 'react-router-dom';
import { AuthContext } from '../Contexts/AuthContext';
import { useSnackbar } from '../Contexts/SnackbarContext';
import { loginUser, registerUser } from '../Services/APIService';

function Login() {
  const { isLoggedIn, login } = useContext(AuthContext);
  const showSnackbar = useSnackbar();
  const [value, setValue] = useState(0);
  const [loginData, setLoginData] = useState({ email: '', password: '' });
  const [registerData, setRegisterData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    location: '',
    phoneNumber: '',
  });
  const [redirectToMenu, setRedirectToMenu] = useState(false);

  const handleChange = (event, newValue) => {
    setValue(newValue);
  };

  const handleLoginChange = (e) => {
    setLoginData({ ...loginData, [e.target.name]: e.target.value });
  };

  const handleRegisterChange = (e) => {
    setRegisterData({ ...registerData, [e.target.name]: e.target.value });
  };

  const handleLocationChange = (e) => {
    setRegisterData({ ...registerData, location: e.target.value });
  };

  const handleLoginSubmit = async (e) => {
    e.preventDefault();
    try {
      const token = await loginUser(loginData.email, loginData.password);
      login(token);
      showSnackbar('You have successfully logged in!', 'success');
      setRedirectToMenu(true);
    } catch (error) {
      console.error('There was an error logging in!', error);
      showSnackbar('Login failed!', 'error');
    }
  };

  const handleRegisterSubmit = async (e) => {
    e.preventDefault();

    const { firstName, lastName, email, password, location, phoneNumber } = registerData;

    if (!firstName || !lastName || !email || !password || !location || !phoneNumber) {
      showSnackbar('Please fill out all fields.!', 'error');
      return;
    }

    try {
      const status = await registerUser(registerData);
      if (status === 200) {
        showSnackbar('User registered successfully', 'success');
        setValue(0);
        setRegisterData({
          firstName: '',
          lastName: '',
          email: '',
          password: '',
          location: '',
          phoneNumber: '',
        });
      }
    } catch (error) {
      showSnackbar("Registration failed!", 'error');
    }
  };


  if (isLoggedIn || redirectToMenu) {
    return <Navigate to="/menu" />;
  }

  return (
    <Container
      disableGutters
      maxWidth={false}
      style={{
        backgroundImage: 'url(cool-background.svg)',
        backgroundSize: 'cover',
        backgroundPosition: 'center',
        backgroundRepeat: 'no-repeat',
        visibility: isLoggedIn ? 'hidden' : 'visible',
        width: "100%",
        height: "100vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        margin: 0,
        padding: 0,
        overflow: "hidden"
      }}
    >
      <Box
        component="main"
        sx={{
          width: '100%',
          maxWidth: '400px',
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          backgroundColor: '#fff',
          padding: 4,
          borderRadius: 2,
          boxShadow: 3,
        }}
      >
        <Box
          sx={{
            width: '100%',
            textAlign: 'center',
          }}
        >
          <Tabs
            value={value}
            onChange={handleChange}
            indicatorColor="primary"
            textColor="primary"
            variant="fullWidth"
          >
            <Tab label="Login" />
            <Tab label="Sign Up" />
          </Tabs>

          <Box sx={{ my: 2 }}>
            <Avatar sx={{ m: 1, bgcolor: 'secondary.main', margin: '0 auto' }}>
              <LockOutlinedIcon />
            </Avatar>
          </Box>

          {value === 0 ? (
            <Box component="form" sx={{ mt: 1 }} onSubmit={handleLoginSubmit}>
              <TextField
                margin="dense"
                required
                fullWidth
                id="login"
                label="Login"
                name="email"
                autoComplete="email"
                autoFocus
                value={loginData.email}
                onChange={handleLoginChange}
              />
              <TextField
                margin="dense"
                required
                fullWidth
                name="password"
                label="Password"
                type="password"
                id="password"
                autoComplete="current-password"
                value={loginData.password}
                onChange={handleLoginChange}
              />
              <Button
                type="submit"
                fullWidth
                variant="contained"
                sx={{ mt: 3, mb: 2 }}
              >
                Log In
              </Button>
              <Box sx={{ mt: 2 }}>
                <Link href="#" variant="body2">
                  Forgot Password?
                </Link>
              </Box>
            </Box>
          ) : (
            <Box component="form" sx={{ mt: 1 }} onSubmit={handleRegisterSubmit}>
              <TextField
                margin="dense"
                required
                fullWidth
                id="firstName"
                label="First Name"
                name="firstName"
                autoComplete="given-name"
                autoFocus
                value={registerData.firstName}
                onChange={handleRegisterChange}
              />
              <TextField
                margin="dense"
                required
                fullWidth
                id="lastName"
                label="Last Name"
                name="lastName"
                autoComplete="family-name"
                value={registerData.lastName}
                onChange={handleRegisterChange}
              />
              <TextField
                margin="dense"
                required
                fullWidth
                id="email"
                label="Email Address"
                name="email"
                autoComplete="email"
                value={registerData.email}
                onChange={handleRegisterChange}
              />
              <TextField
                margin="dense"
                required
                fullWidth
                name="password"
                label="Password"
                type="password"
                id="password"
                autoComplete="new-password"
                value={registerData.password}
                onChange={handleRegisterChange}
              />
              <Select
                margin="dense"
                required
                fullWidth
                id="location"
                name="location"
                value={registerData.location}
                onChange={handleLocationChange}
                displayEmpty
              >
                <MenuItem value="" disabled>Select Location</MenuItem>
                {[...Array(22)].map((_, index) => {
                  const value = `T${index + 1}`;
                  return (
                    <MenuItem key={value} value={value}>
                      {value}
                    </MenuItem>
                  );
                })}
              </Select>
              <TextField
                margin="dense"
                required
                fullWidth
                id="phoneNumber"
                label="Phone Number"
                name="phoneNumber"
                autoComplete="tel"
                value={registerData.phoneNumber}
                onChange={handleRegisterChange}
              />
              <Button
                type="submit"
                fullWidth
                variant="contained"
                sx={{ mt: 3, mb: 2 }}
              >
                Sign Up
              </Button>
            </Box>
          )}
        </Box>
      </Box>
    </Container>
  );
}

export default Login;
