import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Pesticides } from './pesticides';

describe('Pesticides', () => {
  let component: Pesticides;
  let fixture: ComponentFixture<Pesticides>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Pesticides],
    }).compileComponents();

    fixture = TestBed.createComponent(Pesticides);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
