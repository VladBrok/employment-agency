if (
  !localStorage.getItem("token") ||
  Date.now() - localStorage.getItem("expires") >= 600000
) {
  location.replace("./login.html");
}
