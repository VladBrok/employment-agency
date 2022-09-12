const isDev = true; // It changes via the publish.sh script
const apiUrls = isDev
  ? ["https://localhost:7288/api"]
  : [
      "https://bsite.net/employmentAgencyApi/api",
      "http://www.employmentagencyapi.somee.com/api",
    ];

export { apiUrls };
export { isDev };
