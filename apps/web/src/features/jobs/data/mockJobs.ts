import type { MockJob } from "@/features/jobs/types/job.types";

export const mockJobs: MockJob[] = [
  {
    id: "1",
    title: "Senior Product Designer",
    employer: "Monzo Bank",
    location: "London",
    postcode: "EC2A 4DP",
    source: "LinkedIn",
    summary:
      "We're looking for an experienced product designer to join our Cards team. You'll work on features used by millions of customers daily, focusing on user research, interaction design, and visual craft.",
    postedDate: "2h ago",
    freshness: "fresh",
    featured: true,
    url: "https://example.com",
    salary: "GBP 75,000 - 95,000",
    type: "Full-time"
  },
  {
    id: "2",
    title: "Frontend Engineer",
    employer: "Deliveroo",
    location: "London",
    postcode: "EC2A 3AY",
    source: "Indeed",
    summary:
      "Join our engineering team to build delightful web experiences for our restaurant partners. Strong React, TypeScript, and testing experience required.",
    postedDate: "5h ago",
    freshness: "fresh",
    url: "https://example.com",
    salary: "GBP 65,000 - 85,000",
    type: "Full-time"
  },
  {
    id: "3",
    title: "Data Scientist - ML",
    employer: "Octopus Energy",
    location: "London",
    postcode: "EC1V 2NX",
    source: "Company Site",
    summary:
      "Help us use machine learning to revolutionize the energy industry. Work on demand forecasting, pricing optimization, and smart grid technologies.",
    postedDate: "1d ago",
    freshness: "recent",
    url: "https://example.com",
    salary: "GBP 70,000 - 90,000",
    type: "Full-time"
  },
  {
    id: "4",
    title: "Product Manager",
    employer: "Revolut",
    location: "London",
    postcode: "E14 5AB",
    source: "LinkedIn",
    summary:
      "Lead product strategy for our business banking vertical. You'll define roadmaps, work with cross-functional teams, and ship features that help SMEs manage their finances.",
    postedDate: "1d ago",
    freshness: "recent",
    url: "https://example.com",
    salary: "GBP 80,000 - 110,000",
    type: "Full-time"
  },
  {
    id: "5",
    title: "UX Researcher",
    employer: "GDS",
    location: "London",
    postcode: "SW1A 2AA",
    source: "Civil Service Jobs",
    summary:
      "Conduct research to improve digital government services used by millions. Mix of qualitative and quantitative methods, working with diverse user groups.",
    postedDate: "2d ago",
    freshness: "recent",
    url: "https://example.com",
    salary: "GBP 45,000 - 58,000",
    type: "Full-time"
  },
  {
    id: "6",
    title: "Backend Engineer - Go",
    employer: "Cloudflare",
    location: "London",
    postcode: "EC4Y 8EN",
    source: "Company Site",
    summary:
      "Build distributed systems that power the internet's infrastructure. Work on high-performance services handling millions of requests per second.",
    postedDate: "3d ago",
    freshness: "older",
    url: "https://example.com",
    salary: "GBP 70,000 - 100,000",
    type: "Full-time"
  },
  {
    id: "7",
    title: "Content Designer",
    employer: "Spotify",
    location: "London",
    postcode: "W1F 8FD",
    source: "LinkedIn",
    summary:
      "Shape product experiences through words. Write clear, concise copy that helps millions of users discover and enjoy music and podcasts.",
    postedDate: "3d ago",
    freshness: "older",
    url: "https://example.com",
    salary: "GBP 50,000 - 65,000",
    type: "Full-time"
  },
  {
    id: "8",
    title: "Engineering Manager",
    employer: "Stripe",
    location: "London",
    postcode: "EC2A 4SD",
    source: "Indeed",
    summary:
      "Lead a team of engineers building payment infrastructure for the internet. Focus on technical direction, team growth, and delivery excellence.",
    postedDate: "4d ago",
    freshness: "older",
    url: "https://example.com",
    salary: "GBP 90,000 - 130,000",
    type: "Full-time"
  }
];

export function getMockJobById(jobId: string): MockJob | undefined {
  return mockJobs.find((job) => job.id === jobId);
}

export function searchMockJobs(keyword: string, postcode: string): MockJob[] {
  const normalizedKeyword = keyword.trim().toLowerCase();
  const normalizedPostcode = postcode.trim().toLowerCase();

  if (!normalizedKeyword && !normalizedPostcode) {
    return mockJobs;
  }

  return mockJobs.filter((job) => {
    const matchesKeyword =
      !normalizedKeyword ||
      job.title.toLowerCase().includes(normalizedKeyword) ||
      job.summary.toLowerCase().includes(normalizedKeyword) ||
      job.employer.toLowerCase().includes(normalizedKeyword);

    const matchesPostcode =
      !normalizedPostcode ||
      job.postcode.toLowerCase().includes(normalizedPostcode) ||
      job.location.toLowerCase().includes(normalizedPostcode);

    return matchesKeyword && matchesPostcode;
  });
}
