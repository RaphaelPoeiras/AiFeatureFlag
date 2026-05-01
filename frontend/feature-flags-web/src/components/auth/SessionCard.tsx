import { initialsFromEmail } from "../../utils/initials";

type SessionCardProps = {
  email: string;
};

export function SessionCard({ email }: SessionCardProps) {
  return (
    <div className="session-summary">
      <div className="avatar" aria-hidden>
        {initialsFromEmail(email)}
      </div>
      <div>
        <div className="session-email">{email}</div>
        <p className="session-note">JWT stored in localStorage for this demo UI only.</p>
      </div>
    </div>
  );
}
