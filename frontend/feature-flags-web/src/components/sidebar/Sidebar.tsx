import type { ReactNode } from "react";

type SidebarProps = {
  authTitle: string;
  authContent: ReactNode;
  catalogContent: ReactNode;
};

export function Sidebar({ authTitle, authContent, catalogContent }: SidebarProps) {
  return (
    <aside>
      <div className="card">
        <div className="card__header">
          <h2 className="card__title">{authTitle}</h2>
        </div>
        <div className="card__body">{authContent}</div>

        <div className="divider" />

        <div className="card__header">
          <h2 className="card__title">Public catalog</h2>
        </div>
        <div className="card__body card__body--flush-top">{catalogContent}</div>
      </div>
    </aside>
  );
}
