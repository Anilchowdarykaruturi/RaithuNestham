import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Fertilizers } from './fertilizers';

describe('Fertilizers', () => {
  let component: Fertilizers;
  let fixture: ComponentFixture<Fertilizers>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Fertilizers],
    }).compileComponents();

    fixture = TestBed.createComponent(Fertilizers);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
