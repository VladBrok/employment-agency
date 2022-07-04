const isDev = false; // It changes via the publish.sh script
const apiUrl = isDev
  ? "https://localhost:7288/api"
  : "https://bsite.net/employmentAgencyApi/api";
export { apiUrl };
